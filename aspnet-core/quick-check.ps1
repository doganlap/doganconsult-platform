$Server = "46.224.64.95"
$Pass = "As`$123456"

Write-Host "Checking server status..." -ForegroundColor Cyan

# Check files
Write-Host "`n[Files on server]" -ForegroundColor Yellow
wsl bash -c "sshpass -p '$Pass' ssh -o StrictHostKeyChecking=no root@$Server 'ls -la /opt/doganconsult/'"

# Check Docker containers
Write-Host "`n[Docker containers]" -ForegroundColor Yellow
wsl bash -c "sshpass -p '$Pass' ssh root@$Server 'docker ps'"

# Check if services are accessible
Write-Host "`n[Testing endpoints]" -ForegroundColor Yellow
$urls = @("5000", "5001", "5002")
foreach ($port in $urls) {
    try {
        $response = Invoke-WebRequest "http://${Server}:${port}" -TimeoutSec 3 -ErrorAction SilentlyContinue
        Write-Host "  Port $port : ONLINE" -ForegroundColor Green
    } catch {
        Write-Host "  Port $port : OFFLINE" -ForegroundColor Red
    }
}
