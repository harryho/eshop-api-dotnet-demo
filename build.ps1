#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build the Eshop API project using .NET 8 SDK
.DESCRIPTION
    This script ensures the .NET 8 SDK is active and builds the solution.
.PARAMETER Clean
    Clean before building
.PARAMETER Release
    Build in Release configuration (default is Debug)
.EXAMPLE
    .\build.ps1
    Build the solution in Debug mode
.EXAMPLE
    .\build.ps1 -Clean
    Clean and then build
.EXAMPLE
    .\build.ps1 -Release
    Build in Release configuration
#>

[CmdletBinding()]
param(
    [switch]$Clean,
    [switch]$Release
)

Write-Host "🔧 Eshop API Builder" -ForegroundColor Cyan
Write-Host "====================" -ForegroundColor Cyan
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

# Clean if requested
if ($Clean) {
    Write-Host "🧹 Cleaning solution..." -ForegroundColor Cyan
    & dotnet clean
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Clean failed with exit code: $LASTEXITCODE" -ForegroundColor Red
        exit $LASTEXITCODE
    }
    Write-Host ""
}

# Build
$configuration = if ($Release) { "Release" } else { "Debug" }
Write-Host "🔨 Building solution ($configuration)..." -ForegroundColor Cyan

$buildArgs = @("build")
if ($Release) {
    $buildArgs += "--configuration", "Release"
}

& dotnet $buildArgs

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✅ Build successful!" -ForegroundColor Green
    exit 0
} else {
    Write-Host ""
    Write-Host "❌ Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit $LASTEXITCODE
}
