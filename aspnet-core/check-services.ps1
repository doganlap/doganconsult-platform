# DoganConsult Platform - Service Health Check
# This script checks the status of all services

$ErrorActionPreference = "Continue"

Write-Host "`n=== DoganConsult Platform - Service Health Check ===" -ForegroundColor Cyan
Write-Host "Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

# Service configuration
$services = @(
    @{Name="Identity"; Port=5002; Url="http://localhost:5002"},
    @{Name="Organization"; Port=5003; Url="http://localhost:5003"},
    @{Name="Workspace"; Port=5004; Url="http://localhost:5004"},
    @{Name="UserProfile"; Port=5005; Url="http://localhost:5005"},
    @{Name="Audit"; Port=5006; Url="http://localhost:5006"},
    @{Name="Document"; Port=5007; Url="http://localhost:5007"},
    @{Name="AI"; Port=5008; Url="http://localhost:5008"},
    @{Name="Gateway"; Port=5000; Url="http://localhost:5000"},
    @{Name="Blazor UI"; Port=5001; Url="http://localhost:5001"}
)

# Check port status
Write-Host "[Port Status Check]" -ForegroundColor Yellow
$runningCount = 0
$stoppedCount = 0

foreach ($service in $services) {
    $port = $service.Port
    $conn = Test-NetConnection -ComputerName localhost -Port $port -InformationLevel Quiet -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    
    if ($conn) {
        Write-Host "  ✓ $($service.Name.PadRight(20)) (port $port) - RUNNING" -ForegroundColor Green
        $runningCount++
    } else {
        Write-Host "  ✗ $($service.Name.PadRight(20)) (port $port) - STOPPED" -ForegroundColor Red
        $stoppedCount++
    }
}

Write-Host ""

# Check HTTP endpoints for running services
Write-Host "[HTTP Endpoint Check]" -ForegroundColor Yellow
$healthyCount = 0
$unhealthyCount = 0

foreach ($service in $services) {
    $port = $service.Port
    $conn = Test-NetConnection -ComputerName localhost -Port $port -InformationLevel Quiet -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    
    if ($conn) {
        try {
            $response = Invoke-WebRequest -Uri $service.Url -Method Get -TimeoutSec 5 -UseBasicParsing -ErrorAction SilentlyContinue
            if ($response.StatusCode -eq 200 -or $response.StatusCode -eq 404) {
                Write-Host "  ✓ $($service.Name.PadRight(20)) - HTTP OK" -ForegroundColor Green
                $healthyCount++
            } else {
                Write-Host "  ⚠ $($service.Name.PadRight(20)) - HTTP $($response.StatusCode)" -ForegroundColor Yellow
                $unhealthyCount++
            }
        } catch {
            Write-Host "  ⚠ $($service.Name.PadRight(20)) - HTTP Error: $($_.Exception.Message)" -ForegroundColor Yellow
            $unhealthyCount++
        }
    }
}

Write-Host ""

# Check dotnet processes
Write-Host "[Process Status]" -ForegroundColor Yellow
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($dotnetProcesses) {
    Write-Host "  ✓ Found $($dotnetProcesses.Count) dotnet process(es) running" -ForegroundColor Green
    foreach ($proc in $dotnetProcesses) {
        $memoryMB = [math]::Round($proc.WorkingSet64 / 1MB, 2)
        Write-Host "    - PID $($proc.Id): $memoryMB MB" -ForegroundColor Gray
    }
} else {
    Write-Host "  ✗ No dotnet processes found" -ForegroundColor Red
}

# Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Summary:" -ForegroundColor White
Write-Host "  Services Running:  $runningCount / $($services.Count)" -ForegroundColor $(if ($runningCount -eq $services.Count) { "Green" } else { "Yellow" })
Write-Host "  Services Stopped:  $stoppedCount / $($services.Count)" -ForegroundColor $(if ($stoppedCount -eq 0) { "Green" } else { "Red" })
Write-Host "  HTTP Healthy:      $healthyCount / $runningCount" -ForegroundColor $(if ($healthyCount -eq $runningCount) { "Green" } else { "Yellow" })

if ($runningCount -eq $services.Count -and $healthyCount -eq $runningCount) {
    Write-Host "`n✅ Platform is HEALTHY!" -ForegroundColor Green
    Write-Host "`n📱 Access URLs:" -ForegroundColor Cyan
    Write-Host "   🌐 Blazor UI:  http://localhost:5001" -ForegroundColor White
    Write-Host "   🔧 Gateway:    http://localhost:5000" -ForegroundColor White
    Write-Host "   🔐 Identity:   http://localhost:5002" -ForegroundColor White
} elseif ($runningCount -gt 0) {
    Write-Host "`n⚠ Platform is PARTIALLY RUNNING" -ForegroundColor Yellow
    Write-Host "Check the PowerShell windows for error messages" -ForegroundColor Gray
} else {
    Write-Host "`n❌ Platform is NOT RUNNING" -ForegroundColor Red
    Write-Host "Run: .\start-all-services.ps1 to start" -ForegroundColor Gray
}

Write-Host "========================================`n" -ForegroundColor Cyan

