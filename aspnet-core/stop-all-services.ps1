# DoganConsult Platform - Stop All Services
# This script stops all running dotnet services gracefully

$ErrorActionPreference = "Continue"

Write-Host "`n=== DoganConsult Platform - Stopping All Services ===" -ForegroundColor Yellow
Write-Host "Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

# Find all dotnet processes
$dotnetProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue

if ($dotnetProcesses) {
    Write-Host "Found $($dotnetProcesses.Count) dotnet process(es) running" -ForegroundColor Cyan
    Write-Host "Stopping services..." -ForegroundColor Yellow
    Write-Host ""
    
    # Try graceful shutdown first
    foreach ($process in $dotnetProcesses) {
        try {
            Write-Host "  Stopping process $($process.Id)..." -ForegroundColor Gray
            $process.CloseMainWindow() | Out-Null
            Start-Sleep -Milliseconds 500
        } catch {
            # Ignore errors during graceful shutdown
        }
    }
    
    Start-Sleep -Seconds 2
    
    # Force kill any remaining processes
    $remainingProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
    if ($remainingProcesses) {
        Write-Host "  Force stopping $($remainingProcesses.Count) remaining process(es)..." -ForegroundColor Yellow
        $remainingProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
        Start-Sleep -Seconds 1
    }
    
    Write-Host "`n✓ All services stopped" -ForegroundColor Green
} else {
    Write-Host "No dotnet services are currently running" -ForegroundColor Gray
}

# Verify all services are stopped
Write-Host "`nVerifying all services stopped..." -ForegroundColor Yellow
$ports = @(5000, 5001, 5002, 5003, 5004, 5005, 5006, 5007, 5008)
$allStopped = $true

foreach ($port in $ports) {
    $conn = Test-NetConnection -ComputerName localhost -Port $port -InformationLevel Quiet -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($conn) {
        Write-Host "  ⚠ Port $port is still in use" -ForegroundColor Yellow
        $allStopped = $false
    }
}

if ($allStopped) {
    Write-Host "✓ All ports are free" -ForegroundColor Green
} else {
    Write-Host "`n⚠ Some ports are still in use. They may be used by other applications." -ForegroundColor Yellow
    Write-Host "  If needed, wait 10 seconds for TCP timeout and try again." -ForegroundColor Gray
}

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host "Services stopped successfully!" -ForegroundColor Green
Write-Host "To start again, run: .\start-all-services.ps1" -ForegroundColor Gray
Write-Host "========================================`n" -ForegroundColor Cyan

