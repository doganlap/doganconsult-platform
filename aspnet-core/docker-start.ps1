# DoganConsult Platform - Docker Build and Start
# This script builds and starts all services in Docker

$ErrorActionPreference = "Stop"

Write-Host "`n=== DoganConsult Platform - Docker Deployment ===" -ForegroundColor Green
Write-Host "Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

Push-Location $PSScriptRoot

# Step 1: Stop existing containers
Write-Host "[Step 1/4] Stopping existing containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.local.yml down 2>&1 | Out-Null
Write-Host "  ✓ Existing containers stopped" -ForegroundColor Green

# Step 2: Build images
Write-Host "`n[Step 2/4] Building Docker images..." -ForegroundColor Yellow
Write-Host "  This will take 5-10 minutes..." -ForegroundColor Gray
$buildOutput = docker-compose -f docker-compose.local.yml build 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ All images built successfully" -ForegroundColor Green
} else {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    Write-Host "`nBuild output (last 20 lines):" -ForegroundColor Yellow
    $buildOutput | Select-Object -Last 20 | ForEach-Object { Write-Host "    $_" }
    Pop-Location
    exit 1
}

# Step 3: Start containers
Write-Host "`n[Step 3/4] Starting containers..." -ForegroundColor Yellow
docker-compose -f docker-compose.local.yml up -d

if ($LASTEXITCODE -eq 0) {
    Write-Host "  ✓ All containers started" -ForegroundColor Green
} else {
    Write-Host "  ✗ Failed to start containers!" -ForegroundColor Red
    Pop-Location
    exit 1
}

# Step 4: Wait and check status
Write-Host "`n[Step 4/4] Waiting for services to initialize..." -ForegroundColor Yellow
Write-Host "  This will take about 30 seconds..." -ForegroundColor Gray
Start-Sleep -Seconds 30

Write-Host "`nChecking container status..." -ForegroundColor Yellow
$containers = docker-compose -f docker-compose.local.yml ps --format json | ConvertFrom-Json
$runningCount = 0
$stoppedCount = 0

foreach ($container in $containers) {
    $name = $container.Service
    $status = $container.State
    
    if ($status -eq "running") {
        Write-Host "  ✓ $name - RUNNING" -ForegroundColor Green
        $runningCount++
    } else {
        Write-Host "  ✗ $name - $status" -ForegroundColor Red
        $stoppedCount++
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
    Write-Host "   docker-compose -f docker-compose.local.yml logs -f" -ForegroundColor White
}

Write-Host "`n📋 Useful commands:" -ForegroundColor Cyan
Write-Host "   View logs:    docker-compose -f docker-compose.local.yml logs -f" -ForegroundColor Gray
Write-Host "   Stop all:     docker-compose -f docker-compose.local.yml down" -ForegroundColor Gray
Write-Host "   Restart:      docker-compose -f docker-compose.local.yml restart" -ForegroundColor Gray
Write-Host "========================================`n" -ForegroundColor Cyan

