# Easy Setup Script - Standardized Customer Deployment
# This script provides an easy way to deploy DoganConsult Platform

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Local", "Docker", "Server")]
    [string]$DeploymentType = "Local",
    
    [Parameter(Mandatory=$false)]
    [string]$ServerIP = "",
    
    [Parameter(Mandatory=$false)]
    [string]$ServerUser = "root"
)

$ErrorActionPreference = "Stop"

Write-Host @"
╔═══════════════════════════════════════════════════════════╗
║     DoganConsult Platform - Easy Setup Installer          ║
╚═══════════════════════════════════════════════════════════╝
"@ -ForegroundColor Cyan

Write-Host "`nDeployment Type: $DeploymentType" -ForegroundColor Yellow

# Check prerequisites
Write-Host "`n[1/4] Checking prerequisites..." -ForegroundColor Yellow

# Check .NET SDK
$dotnetVersion = dotnet --version 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ .NET SDK not found. Please install .NET 10.0 SDK" -ForegroundColor Red
    Write-Host "    Download from: https://dotnet.microsoft.com/download" -ForegroundColor Gray
    exit 1
}
Write-Host "  ✓ .NET SDK: $dotnetVersion" -ForegroundColor Green

# Check Docker (if Docker deployment)
if ($DeploymentType -eq "Docker") {
    $dockerVersion = docker --version 2>&1
    if ($LASTEXITCODE -ne 0) {
        Write-Host "  ✗ Docker not found. Please install Docker Desktop" -ForegroundColor Red
        Write-Host "    Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Gray
        exit 1
    }
    Write-Host "  ✓ Docker: $dockerVersion" -ForegroundColor Green
}

# Build solution
Write-Host "`n[2/4] Building solution..." -ForegroundColor Yellow
Push-Location $PSScriptRoot
dotnet build DoganConsult.Platform.sln -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Build successful" -ForegroundColor Green

# Deploy based on type
switch ($DeploymentType) {
    "Local" {
        Write-Host "`n[3/4] Setting up local deployment..." -ForegroundColor Yellow
        Write-Host "  Starting services locally..." -ForegroundColor Gray
        Write-Host "`n[4/4] Starting services..." -ForegroundColor Yellow
        Write-Host "  Run '.\start-local.ps1' to start all services" -ForegroundColor Cyan
        Write-Host "  Or run this script with: .\easy-setup.ps1 -DeploymentType Local" -ForegroundColor Cyan
        
        # Optionally start services
        $start = Read-Host "`nStart services now? (Y/N)"
        if ($start -eq "Y" -or $start -eq "y") {
            & "$PSScriptRoot\start-local.ps1"
        }
    }
    
    "Docker" {
        Write-Host "`n[3/4] Building Docker images..." -ForegroundColor Yellow
        docker-compose -f docker-compose.local.yml build
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  ✗ Docker build failed!" -ForegroundColor Red
            exit 1
        }
        Write-Host "  ✓ Docker images built" -ForegroundColor Green
        
        Write-Host "`n[4/4] Starting Docker containers..." -ForegroundColor Yellow
        docker-compose -f docker-compose.local.yml up -d
        if ($LASTEXITCODE -ne 0) {
            Write-Host "  ✗ Failed to start containers!" -ForegroundColor Red
            exit 1
        }
        Write-Host "  ✓ Services started in Docker" -ForegroundColor Green
        
        Write-Host "`n=== Deployment Complete ===" -ForegroundColor Green
        Write-Host "`nAccess URLs:" -ForegroundColor Cyan
        Write-Host "  Blazor UI: http://localhost:5001" -ForegroundColor White
        Write-Host "  API Gateway: http://localhost:5000" -ForegroundColor White
        Write-Host "`nTo stop: docker-compose -f docker-compose.local.yml down" -ForegroundColor Yellow
        Write-Host "To view logs: docker-compose -f docker-compose.local.yml logs -f" -ForegroundColor Yellow
    }
    
    "Server" {
        if ([string]::IsNullOrEmpty($ServerIP)) {
            Write-Host "  ✗ Server IP is required for server deployment" -ForegroundColor Red
            Write-Host "    Usage: .\easy-setup.ps1 -DeploymentType Server -ServerIP '46.4.206.15'" -ForegroundColor Gray
            exit 1
        }
        
        Write-Host "`n[3/4] Preparing server deployment..." -ForegroundColor Yellow
        Write-Host "  Server: $ServerUser@$ServerIP" -ForegroundColor Gray
        
        Write-Host "`n[4/4] Deploying to server..." -ForegroundColor Yellow
        & "$PSScriptRoot\deploy-production.ps1" -ServerIP $ServerIP -ServerUser $ServerUser
    }
}

Pop-Location

Write-Host "`n=== Setup Complete ===" -ForegroundColor Green
Write-Host "`nFor more information, see:" -ForegroundColor Cyan
Write-Host "  - DEPLOYMENT_STATUS.md" -ForegroundColor White
Write-Host "  - README.md" -ForegroundColor White

