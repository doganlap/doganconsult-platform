# Check Deployment Status and Troubleshoot
param(
    [string]$Server = "46.224.64.95",
    [string]$User = "root",
    [string]$Pass = "As`$123456"
)

Write-Host "=== Deployment Status Check ===" -ForegroundColor Cyan
Write-Host "Server: $Server" -ForegroundColor Yellow
Write-Host ""

# Check connection
Write-Host "[1] Testing connection..." -ForegroundColor Green
try {
    $info = wsl bash -c "sshpass -p '$Pass' ssh -o StrictHostKeyChecking=no $User@$Server 'hostname && uptime'"
    Write-Host $info -ForegroundColor Gray
    Write-Host "  ✓ Server reachable" -ForegroundColor Green
} catch {
    Write-Host "  ✗ Cannot connect to server" -ForegroundColor Red
    exit 1
}

# Check files
Write-Host ""
Write-Host "[2] Checking deployed files..." -ForegroundColor Green
$files = wsl bash -c "sshpass -p '$Pass' ssh $User@$Server 'ls -lah /opt/doganconsult'"
Write-Host $files -ForegroundColor Gray

# Check Docker
Write-Host ""
Write-Host "[3] Checking Docker containers..." -ForegroundColor Green
$containers = wsl bash -c "sshpass -p '$Pass' ssh $User@$Server 'cd /opt/doganconsult && docker compose ps'"
Write-Host $containers -ForegroundColor Gray

# Check services status
Write-Host ""
Write-Host "[4] Testing service endpoints..." -ForegroundColor Green

$services = @{
    "API Gateway" = "5000"
    "Blazor UI" = "5001"
    "Identity" = "5002"
    "Organization" = "5003"
    "Workspace" = "5004"
    "UserProfile" = "5005"
    "Audit" = "5006"
    "Document" = "5007"
    "AI Service" = "5008"
}

foreach ($service in $services.Keys) {
    $port = $services[$service]
    try {
        $response = Invoke-WebRequest -Uri "http://${Server}:${port}" -TimeoutSec 5 -ErrorAction SilentlyContinue
        Write-Host "  ✓ $service (port $port) - Running" -ForegroundColor Green
    } catch {
        Write-Host "  ✗ $service (port $port) - Not responding" -ForegroundColor Red
    }
}

# Check logs for errors
Write-Host ""
Write-Host "[5] Checking recent logs for errors..." -ForegroundColor Green
$logs = wsl bash -c "sshpass -p '$Pass' ssh $User@$Server 'cd /opt/doganconsult && docker compose logs --tail=50 | grep -i error || echo No errors found'"
Write-Host $logs -ForegroundColor Gray

# Summary
Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Platform URLs:" -ForegroundColor White
Write-Host "  Main UI: http://$Server:5001" -ForegroundColor Yellow
Write-Host "  API: http://$Server:5000" -ForegroundColor Yellow
Write-Host ""
Write-Host "Troubleshooting commands:" -ForegroundColor White
Write-Host "  View logs: ssh $User@$Server 'cd /opt/doganconsult && docker compose logs -f'" -ForegroundColor Cyan
Write-Host "  Restart: ssh $User@$Server 'cd /opt/doganconsult && docker compose restart'" -ForegroundColor Cyan
Write-Host "  Rebuild: ssh $User@$Server 'cd /opt/doganconsult && docker compose down && docker compose build && docker compose up -d'" -ForegroundColor Cyan
Write-Host ""

Read-Host "Press Enter to close"
