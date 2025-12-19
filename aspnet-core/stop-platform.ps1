# Stop All Platform Services

Write-Host "Stopping DoganConsult Platform services..." -ForegroundColor Yellow

# Kill dotnet processes on our ports
$ports = @(5000, 5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008)
$killed = 0

foreach ($port in $ports) {
    try {
        $connections = Get-NetTCPConnection -LocalPort $port -ErrorAction SilentlyContinue
        foreach ($conn in $connections) {
            if ($conn.OwningProcess) {
                Stop-Process -Id $conn.OwningProcess -Force -ErrorAction SilentlyContinue
                $killed++
            }
        }
    } catch {
        # Port might not be in use
    }
}

# Also kill any dotnet processes running our DLLs
$processes = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Where-Object {
    $_.CommandLine -like "*DoganConsult*"
}

foreach ($proc in $processes) {
    try {
        Stop-Process -Id $proc.Id -Force -ErrorAction SilentlyContinue
        $killed++
    } catch {
        # Process might already be stopped
    }
}

if ($killed -gt 0) {
    Write-Host "✓ Stopped $killed service(s)" -ForegroundColor Green
} else {
    Write-Host "No running services found" -ForegroundColor Gray
}

Write-Host "Done!" -ForegroundColor Green

