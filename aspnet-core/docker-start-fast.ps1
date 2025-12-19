# DoganConsult Platform - Fast Docker Start (Uses Pre-built Binaries)
# This script uses the already published binaries - NO BUILD NEEDED!

$ErrorActionPreference = "Stop"

Write-Host "`n=== DoganConsult Platform - Fast Docker Start ===" -ForegroundColor Green
Write-Host "Using pre-built binaries from publish/ folder" -ForegroundColor Gray
Write-Host ""

Push-Location $PSScriptRoot

# Check if publish folder exists
if (-not (Test-Path "publish")) {
    Write-Host "❌ publish/ folder not found!" -ForegroundColor Red
    Write-Host "`nYou need to build first. Run:" -ForegroundColor Yellow
    Write-Host "  .\deploy-production.ps1 -SkipDeploy" -ForegroundColor White
    Write-Host "  OR" -ForegroundColor Yellow
    Write-Host "  dotnet publish DoganConsult.Platform.sln -c Release" -ForegroundColor White
    Pop-Location
    exit 1
}

# Step 1: Stop existing containers
Write-Host "[Step 1/3] Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.fast.yml down 2>&1 | Out-Null
Write-Host "  ✓ Existing containers stopped" -ForegroundColor Green

# Step 2: Start containers (no build needed!)
Write-Host "`n[Step 2/3] Starting containers..." -ForegroundColor Yellow
Write-Host "  Using pre-built binaries - this is FAST!" -ForegroundColor Gray

$startOutput = docker-compose -f docker-compose.fast.yml up -d 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ All containers started" -ForegroundColor Green
} else {
    Write-Host "  ✗ Failed to start containers!" -ForegroundColor Red
    Write-Host "`nError output:" -ForegroundColor Yellow
    $startOutput | ForEach-Object { Write-Host "    $_" }
    Pop-Location
    exit 1
}

# Step 3: Wait and check status
Write-Host "`n[Step 3/3] Waiting for services to initialize..." -ForegroundColor Yellow
Write-Host "  This will take about 20 seconds..." -ForegroundColor Gray
Start-Sleep -Seconds 20

Write-Host "`nChecking container status..." -ForegroundColor Yellow
$containers = docker ps --filter "name=dc-" --format "{{.Names}}\t{{.Status}}"
$runningCount = 0

foreach ($line in $containers) {
    if ($line -match "^(dc-\S+)\s+(.+)$") {
        $name = $matches[1]
        $status = $matches[2]
        
        if ($status -like "*Up*") {
            Write-Host "  ✓ $name - RUNNING" -ForegroundColor Green
            $runningCount++
        } else {
            Write-Host "  ✗ $name - $status" -ForegroundColor Red
        }
    }
}

Pop-Location

# Final status
Write-Host "`n========================================" -ForegroundColor Cyan
if ($runningCount -eq 9) {
    Write-Host "🎉 ALL 9 CONTAINERS ARE RUNNING!" -ForegroundColor Green
    Write-Host "`n📱 Access your application:" -ForegroundColor Cyan
    Write-Host "   🌐 Blazor UI:  http://localhost:5001" -ForegroundColor White
    Write-Host "   🔧 Gateway:    http://localhost:5000" -ForegroundColor White
    Write-Host "   🔐 Identity:   http://localhost:5002" -ForegroundColor White
    Write-Host "`n✅ Platform is ready!" -ForegroundColor Green
} else {
    Write-Host "⚠ SOME CONTAINERS FAILED" -ForegroundColor Yellow
    Write-Host "Running: $runningCount / 9" -ForegroundColor Yellow
    Write-Host "`n📋 View logs with:" -ForegroundColor Cyan
    Write-Host "   docker-compose -f docker-compose.fast.yml logs -f" -ForegroundColor White
}

Write-Host "`n📋 Useful commands:" -ForegroundColor Cyan
Write-Host "   View logs:    docker-compose -f docker-compose.fast.yml logs -f" -ForegroundColor Gray
Write-Host "   Stop all:     docker-compose -f docker-compose.fast.yml down" -ForegroundColor Gray
Write-Host "   Restart:      docker-compose -f docker-compose.fast.yml restart" -ForegroundColor Gray
Write-Host "========================================`n" -ForegroundColor Cyan

