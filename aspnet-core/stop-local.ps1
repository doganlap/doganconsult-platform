# Stop All Local Services

Write-Host "Stopping all DoganConsult services..." -ForegroundColor Yellow

$jobs = Get-Job
if ($jobs) {
    $jobs | Stop-Job
    $jobs | Remove-Job
    Write-Host "✓ All services stopped" -ForegroundColor Green
} else {
    Write-Host "No running services found" -ForegroundColor Gray
}

# Kill any remaining dotnet processes on our ports
$ports = @(5000, 5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008)
foreach ($port in $ports) {
    $process = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue | Select-Object -ExpandProperty OwningProcess -Unique
    if ($process) {
        Stop-Process -Id $process -Force -ErrorAction SilentlyContinue
        Write-Host "  Stopped process on port $port" -ForegroundColor Gray
    }
}

Write-Host "Done!" -ForegroundColor Green

