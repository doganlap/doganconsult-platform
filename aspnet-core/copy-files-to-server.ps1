# PowerShell script to copy files to Hetzner server
# Run this in a separate PowerShell window after connecting via PuTTY

$SERVER = "root@46.224.64.95"
$PASSWORD = "aKTbCKAeWapnp9xkLcjF"

Write-Host "======================================" -ForegroundColor Cyan
Write-Host "Copying Files to Server" -ForegroundColor Cyan
Write-Host "======================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "When prompted, enter password: $PASSWORD" -ForegroundColor Yellow
Write-Host ""

# Change to project directory
Set-Location D:\test\aspnet-core

# Copy docker-compose file
Write-Host "1. Copying docker-compose.yml..." -ForegroundColor Green
scp docker-compose.prod.yml "${SERVER}:/opt/doganconsult/docker-compose.yml"

# Copy source directory
Write-Host "2. Copying source files (this may take a few minutes)..." -ForegroundColor Green
scp -r src "${SERVER}:/opt/doganconsult/"

# Copy common.props
Write-Host "3. Copying configuration files..." -ForegroundColor Green
scp common.props "${SERVER}:/opt/doganconsult/"

Write-Host ""
Write-Host "Files copied successfully!" -ForegroundColor Green
Write-Host ""
Write-Host "Now in your PuTTY window, run:" -ForegroundColor Yellow
Write-Host "  cd /opt/doganconsult" -ForegroundColor White
Write-Host "  docker compose build" -ForegroundColor White
Write-Host "  docker compose up -d" -ForegroundColor White
Write-Host ""

Read-Host "Press Enter to close"
