#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Run tests for the Eshop API project using .NET 8 SDK
.DESCRIPTION
    This script ensures the .NET 8 SDK is active and runs the test suite.
    It handles SDK switching via the dn8 alias and provides clear test output.
.PARAMETER NoBuild
    Skip building and only run tests (faster for repeated runs)
.PARAMETER Detailed
    Show detailed test output
.PARAMETER Filter
    Filter tests by name pattern (e.g., "GetAllProducts*")
.PARAMETER Coverage
    Collect code coverage and generate HTML report
.EXAMPLE
    .\scripts\test.ps1
    Run all tests with build
.EXAMPLE
    .\scripts\test.ps1 -NoBuild
    Run tests without rebuilding
.EXAMPLE
    .\scripts\test.ps1 -Filter "GetProductV1*" -Detailed
    Run only tests matching the pattern with detailed output
.EXAMPLE
    .\scripts\test.ps1 -Coverage
    Run tests and generate code coverage report
#>

[CmdletBinding()]
param(
    [switch]$NoBuild,
    [switch]$Detailed,
    [switch]$Coverage,
    [string]$Filter = ""
)

Write-Host "🔧 Eshop API Test Runner" -ForegroundColor Cyan
Write-Host "=========================" -ForegroundColor Cyan
Write-Host ""

# Check for running dotnet processes and stop them
Write-Host "🔍 Checking for running dotnet processes..." -ForegroundColor Cyan
$dotnetProcesses = Get-Process dotnet -ErrorAction SilentlyContinue
if ($dotnetProcesses) {
    Write-Host "   Found $($dotnetProcesses.Count) dotnet process(es)" -ForegroundColor Yellow
    Write-Host "   Stopping dotnet processes..." -ForegroundColor Cyan
    Stop-Process -Name dotnet -Force -ErrorAction SilentlyContinue
    Start-Sleep -Milliseconds 500
    
    # Verify they're stopped
    $remaining = Get-Process dotnet -ErrorAction SilentlyContinue
    if ($remaining) {
        Write-Host "   ⚠️  Warning: Some dotnet processes are still running" -ForegroundColor Yellow
    } else {
        Write-Host "   ✓ All dotnet processes stopped" -ForegroundColor Green
    }
} else {
    Write-Host "   ✓ No dotnet processes running" -ForegroundColor Green
}
Write-Host ""

# Switch to .NET 8 SDK using scoop
Write-Host "🔄 Switching to .NET 8 SDK..." -ForegroundColor Cyan
scoop reset dotnet8-sdk 2>&1 | Out-Null

# Verify .NET version
Write-Host "📌 Checking .NET SDK version..." -ForegroundColor Cyan
$dotnetVersion = dotnet --version
Write-Host "   SDK: $dotnetVersion" -ForegroundColor Green
Write-Host ""

# Build test command
$testArgs = @("test")

if ($NoBuild) {
    $testArgs += "--no-build"
    Write-Host "⚡ Running tests (skipping build)..." -ForegroundColor Cyan
} else {
    Write-Host "🔨 Building and running tests..." -ForegroundColor Cyan
}

if ($Detailed) {
    $testArgs += "--verbosity", "detailed"
}

if ($Filter) {
    $testArgs += "--filter", $Filter
    Write-Host "🔍 Filter: $Filter" -ForegroundColor Cyan
}

if ($Coverage) {
    $testArgs += "--collect:`"XPlat Code Coverage`""
    Write-Host "📊 Coverage collection enabled" -ForegroundColor Cyan
}

Write-Host ""

# Run tests
& dotnet $testArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ All tests passed!" -ForegroundColor Green
    
    # Generate coverage report if requested
    if ($Coverage) {
        Write-Host ""
        Write-Host "📊 Generating coverage report..." -ForegroundColor Cyan
        
        # Find the latest coverage file
        $coverageFile = Get-ChildItem -Path "Eshop.Api.Test/TestResults" -Filter "coverage.cobertura.xml" -Recurse -ErrorAction SilentlyContinue | 
                        Sort-Object LastWriteTime -Descending | 
                        Select-Object -First 1
        
        if ($coverageFile) {
            Write-Host "   Coverage file: $($coverageFile.FullName)" -ForegroundColor Gray
            
            # Check if reportgenerator is installed
            $reportGenExists = dotnet tool list -g | Select-String "dotnet-reportgenerator-globaltool"
            
            if (-not $reportGenExists) {
                Write-Host ""
                Write-Host "⚠️  Installing ReportGenerator tool..." -ForegroundColor Yellow
                & dotnet tool install -g dotnet-reportgenerator-globaltool
            }
            
            # Generate HTML report
            $reportPath = "TestResults/CoverageReport"
            & reportgenerator "-reports:$($coverageFile.FullName)" "-targetdir:$reportPath" "-reporttypes:Html;TextSummary"
            
            if ($LASTEXITCODE -eq 0) {
                Write-Host ""
                Write-Host "✅ Coverage report generated!" -ForegroundColor Green
                Write-Host "   Report location: $reportPath/index.html" -ForegroundColor Cyan
                
                # Display summary if available
                $summaryFile = Join-Path $reportPath "Summary.txt"
                if (Test-Path $summaryFile) {
                    Write-Host ""
                    Write-Host "📈 Coverage Summary:" -ForegroundColor Cyan
                    Get-Content $summaryFile | Write-Host -ForegroundColor Gray
                }
                
                # Ask to open report
                Write-Host ""
                $openReport = Read-Host "Open coverage report in browser? (y/n)"
                if ($openReport -eq 'y' -or $openReport -eq 'Y') {
                    Start-Process (Join-Path $reportPath "index.html")
                }
            } else {
                Write-Host "⚠️  Failed to generate coverage report" -ForegroundColor Yellow
            }
        } else {
            Write-Host "⚠️  Coverage file not found. Make sure coverlet.collector is installed." -ForegroundColor Yellow
        }
    }
    
    exit 0
} else {
    Write-Host ""
    Write-Host "❌ Tests failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
