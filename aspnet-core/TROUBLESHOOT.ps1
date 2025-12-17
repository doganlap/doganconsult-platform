# Troubleshooting Script for DoganConsult Platform
param(
    [string]$Server = "46.224.64.95",
    [string]$User = "root",
    [string]$Pass = "As`$123456",
    [switch]$Fix
)

Write-Host "=== DoganConsult Platform Troubleshooting ===" -ForegroundColor Cyan
Write-Host ""

function Run-SSH {
    param([string]$Command)
    wsl bash -c "sshpass -p '$Pass' ssh -o StrictHostKeyChecking=no $User@$Server '$Command'"
}

# Check 1: Docker
Write-Host "[1] Docker Status" -ForegroundColor Yellow
$dockerStatus = Run-SSH "docker --version && docker info | head -n 5"
Write-Host $dockerStatus -ForegroundColor Gray

# Check 2: Disk Space
Write-Host ""
Write-Host "[2] Disk Space" -ForegroundColor Yellow
$diskSpace = Run-SSH "df -h /opt/doganconsult"
Write-Host $diskSpace -ForegroundColor Gray

# Check 3: Files
Write-Host ""
Write-Host "[3] Deployed Files" -ForegroundColor Yellow
$files = Run-SSH "ls -lh /opt/doganconsult"
Write-Host $files -ForegroundColor Gray

# Check 4: Docker Compose Status
Write-Host ""
Write-Host "[4] Container Status" -ForegroundColor Yellow
$containers = Run-SSH "cd /opt/doganconsult && docker compose ps"
Write-Host $containers -ForegroundColor Gray

# Check 5: Port Availability
Write-Host ""
Write-Host "[5] Port Availability" -ForegroundColor Yellow
$ports = Run-SSH "ss -tulpn | grep -E ':(5000|5001|5002|5003|5004|5005|5006|5007|5008)' || echo 'No services listening'"
Write-Host $ports -ForegroundColor Gray

# Check 6: Recent Errors
Write-Host ""
Write-Host "[6] Recent Errors (last 20 lines)" -ForegroundColor Yellow
$errors = Run-SSH "cd /opt/doganconsult && docker compose logs --tail=20 | grep -i 'error\|exception\|fail' || echo 'No errors found'"
Write-Host $errors -ForegroundColor Gray

# Fix issues if requested
if ($Fix) {
    Write-Host ""
    Write-Host "=== Applying Fixes ===" -ForegroundColor Cyan
    
    Write-Host ""
    Write-Host "[FIX 1] Stopping all containers..." -ForegroundColor Yellow
    Run-SSH "cd /opt/doganconsult && docker compose down"
    
    Write-Host ""
    Write-Host "[FIX 2] Removing old containers and images..." -ForegroundColor Yellow
    Run-SSH "docker system prune -f"
    
    Write-Host ""
    Write-Host "[FIX 3] Rebuilding services..." -ForegroundColor Yellow
    Run-SSH "cd /opt/doganconsult && docker compose build --no-cache --parallel"
    
    Write-Host ""
    Write-Host "[FIX 4] Starting services..." -ForegroundColor Yellow
    Run-SSH "cd /opt/doganconsult && docker compose up -d"
    
    Write-Host ""
    Write-Host "[FIX 5] Checking status..." -ForegroundColor Yellow
    Start-Sleep -Seconds 10
    Run-SSH "cd /opt/doganconsult && docker compose ps"
}

# Recommendations
Write-Host ""
Write-Host "=== Recommendations ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Common Issues & Solutions:" -ForegroundColor White
Write-Host ""
Write-Host "1. Services not starting:" -ForegroundColor Yellow
Write-Host "   - Check logs: .\CHECK-DEPLOYMENT.ps1" -ForegroundColor Gray
Write-Host "   - Rebuild: .\TROUBLESHOOT.ps1 -Fix" -ForegroundColor Gray
Write-Host ""
Write-Host "2. Port conflicts:" -ForegroundColor Yellow
Write-Host "   - Stop conflicting services on server" -ForegroundColor Gray
Write-Host "   - Restart Docker: ssh $User@$Server 'systemctl restart docker'" -ForegroundColor Gray
Write-Host ""
Write-Host "3. Database connection errors:" -ForegroundColor Yellow
Write-Host "   - Verify Railway PostgreSQL is running" -ForegroundColor Gray
Write-Host "   - Check connection strings in docker-compose.prod.yml" -ForegroundColor Gray
Write-Host ""
Write-Host "4. Out of memory:" -ForegroundColor Yellow
Write-Host "   - Check: ssh $User@$Server 'free -h'" -ForegroundColor Gray
Write-Host "   - Reduce parallel builds or increase server RAM" -ForegroundColor Gray
Write-Host ""

Write-Host "Run with -Fix flag to automatically fix common issues:" -ForegroundColor Green
Write-Host "  .\TROUBLESHOOT.ps1 -Fix" -ForegroundColor Cyan
Write-Host ""

Read-Host "Press Enter to close"
