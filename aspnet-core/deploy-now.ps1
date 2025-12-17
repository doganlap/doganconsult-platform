# Complete Deployment Script
param(
    [string]$Server = "46.224.64.95",
    [string]$User = "root",
    [string]$Pass = "As`$123456"
)

$ErrorActionPreference = "Continue"

Write-Host "=== DoganConsult Platform Deployment ===" -ForegroundColor Cyan
Write-Host "Target: $User@$Server" -ForegroundColor Yellow
Write-Host ""

# Step 1: Test connection
Write-Host "[1/6] Testing server connection..." -ForegroundColor Green
try {
    $test = wsl bash -c "sshpass -p '$Pass' ssh -o StrictHostKeyChecking=no $User@$Server 'hostname && docker --version'"
    Write-Host $test -ForegroundColor Gray
    Write-Host "  ✓ Connection successful" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Connection failed" -ForegroundColor Red
    exit 1
}

# Step 2: Create directory
Write-Host ""
Write-Host "[2/6] Creating deployment directory..." -ForegroundColor Green
wsl bash -c "sshpass -p '$Pass' ssh $User@$Server 'mkdir -p /opt/doganconsult/src'"
Write-Host "  ✓ Directory created" -ForegroundColor Green

# Step 3: Transfer docker-compose
Write-Host ""
Write-Host "[3/6] Transferring docker-compose.yml..." -ForegroundColor Green
wsl bash -c "cd /mnt/d/test/aspnet-core && sshpass -p '$Pass' scp docker-compose.prod.yml $User@${Server}:/opt/doganconsult/docker-compose.yml"
Write-Host "  ✓ docker-compose.yml uploaded" -ForegroundColor Green

# Step 4: Transfer common.props
Write-Host ""
Write-Host "[4/6] Transferring common.props..." -ForegroundColor Green
wsl bash -c "cd /mnt/d/test/aspnet-core && sshpass -p '$Pass' scp common.props $User@${Server}:/opt/doganconsult/"
Write-Host "  ✓ common.props uploaded" -ForegroundColor Green

# Step 5: Transfer source
Write-Host ""
Write-Host "[5/6] Transferring source code (this may take a while)..." -ForegroundColor Green
wsl bash -c "cd /mnt/d/test/aspnet-core && sshpass -p '$Pass' scp -r src $User@${Server}:/opt/doganconsult/"
Write-Host "  ✓ Source code uploaded" -ForegroundColor Green

# Step 6: Build and start
Write-Host ""
Write-Host "[6/6] Building and starting services..." -ForegroundColor Green
Write-Host "  This will take 5-10 minutes..." -ForegroundColor Yellow

$deployCmd = "cd /opt/doganconsult && docker compose build --parallel && docker compose up -d && docker compose ps"
wsl bash -c "sshpass -p '$Pass' ssh $User@$Server '$deployCmd'"

Write-Host ""
Write-Host "=== Deployment Complete ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Platform URLs:" -ForegroundColor White
Write-Host "  Blazor UI:    http://$Server:5001" -ForegroundColor Green
Write-Host "  API Gateway:  http://$Server:5000" -ForegroundColor Green
Write-Host "  Identity:     http://$Server:5002" -ForegroundColor Gray
Write-Host ""
Write-Host "Opening platform in browser..." -ForegroundColor Yellow
Start-Process "http://$Server:5001"

Write-Host ""
Write-Host "To check logs:" -ForegroundColor White
Write-Host "  ssh $User@$Server" -ForegroundColor Cyan
Write-Host "  cd /opt/doganconsult" -ForegroundColor Cyan
Write-Host "  docker compose logs -f" -ForegroundColor Cyan
Write-Host ""

Read-Host "Press Enter to close"
