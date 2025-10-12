#!/usr/bin/env pwsh

<#!
.SYNOPSIS
    Drops the existing Eshop database (if present)

#>

[CmdletBinding()]
param (
    [string]$Server   = ${env:MSSQL_SERVER}   ?? 'localhost',
    [string]$Port     = ${env:MSSQL_PORT}     ?? '1433',
    [string]$Username = ${env:MSSQL_USER}     ?? 'sa',
    [string]$Password = ${env:MSSQL_PASSWORD} ?? 'YourStrong@Passw0rd',
    [string]$Database = 'Eshop'
)

$script:UseDocker = $false
$dockerContainerName = 'mssql-infra'

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
$sqlFile = Join-Path -Path $scriptDir -ChildPath 'Eshop.sql'

Write-Host '=========================================='
Write-Host 'Drop Eshop Database'
Write-Host '=========================================='
Write-Host ""
Write-Host "Server: ${Server}:$Port"
Write-Host "Database: $Database"
Write-Host ""


function Test-DockerContainer {
    param (
        [string]$ContainerName
    )

    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        return $false
    }

    try {
        $containers = & docker ps --format '{{.Names}}' 2>$null
    } catch {
        return $false
    }

    return $containers -contains $ContainerName
}

function Initialize-SqlcmdSource {
    if (Get-Command sqlcmd -ErrorAction SilentlyContinue) {
        Write-Host 'Using local sqlcmd'
        $script:UseDocker = $false
        return
    }

    if (Test-DockerContainer -ContainerName $dockerContainerName) {
        Write-Host 'Using sqlcmd from Docker container'
        $script:UseDocker = $true
        return
    }

    Write-Host 'Error: sqlcmd not found and Docker container ''mssql-infra'' is not running'
    Write-Host ''
    Write-Host 'Options:'
    Write-Host '1. Install SQL Server command-line tools'
    Write-Host '2. Run the Docker container: docker-compose up -d mssql'
    exit 1
}

function Invoke-SqlcmdCommand {
    param (
        [string[]]$Arguments,
        [switch]$ReturnOutput,
        [switch]$Silent
    )

    if ($script:UseDocker) {
        $cmd = 'docker'
        $cmdArgs = @('exec', '-i', $dockerContainerName, '/opt/mssql-tools18/bin/sqlcmd', '-C') + $Arguments
    } else {
        $cmd = 'sqlcmd'
        $cmdArgs = $Arguments
    }

    try {
        if ($ReturnOutput) {
            $output = & $cmd @cmdArgs
            if ($LASTEXITCODE -ne 0) {
                throw "Command failed with exit code $LASTEXITCODE"
            }
            return $output
        }

        if ($Silent) {
            & $cmd @cmdArgs > $null 2>&1
        } else {
            & $cmd @cmdArgs
        }

        if ($LASTEXITCODE -ne 0) {
            throw "Command failed with exit code $LASTEXITCODE"
        }
    } catch {
        throw
    }
}

Initialize-SqlcmdSource

Write-Host '1. Testing connection to SQL Server...'
try {
    Invoke-SqlcmdCommand -Arguments @('-S', "$Server,$Port", '-U', $Username, '-P', $Password, '-Q', 'SELECT @@VERSION', '-h', '-1') -Silent
    Write-Host '   ✓ Connected successfully'
} catch {
    Write-Host '   ✗ Failed to connect to SQL Server'
    Write-Host '   Please check your connection parameters'
    exit 1
}

Write-Host ''
Write-Host '2. Dropping existing database (if exists)...'
Invoke-SqlcmdCommand -Arguments @(
    '-S', "$Server,$Port",
    '-U', $Username,
    '-P', $Password,
    '-Q', @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = '$Database')
BEGIN
    ALTER DATABASE [$Database] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$Database];
    PRINT 'Database $Database dropped successfully';
END
ELSE
BEGIN
    PRINT 'Database $Database does not exist';
END
"@,
    '-h', '-1'
)
