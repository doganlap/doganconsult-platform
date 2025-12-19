# Local Deployment Script - Run All Services Locally
# This script starts all DoganConsult services locally for testing

$ErrorActionPreference = "Continue"

Write-Host "=== DoganConsult Platform - Local Deployment ===" -ForegroundColor Green
Write-Host "Starting all services locally..." -ForegroundColor Cyan
Write-Host ""

# Service configuration
$services = @(
    @{Name="Identity"; Port=5002; Path="src/DoganConsult.Identity.HttpApi.Host"; Priority=1},
    @{Name="Organization"; Port=5003; Path="src/DoganConsult.Organization.HttpApi.Host"; Priority=2},
    @{Name="Workspace"; Port=5004; Path="src/DoganConsult.Workspace.HttpApi.Host"; Priority=2},
    @{Name="UserProfile"; Port=5005; Path="src/DoganConsult.UserProfile.HttpApi.Host"; Priority=2},
    @{Name="Audit"; Port=5006; Path="src/DoganConsult.Audit.HttpApi.Host"; Priority=2},
    @{Name="Document"; Port=5007; Path="src/DoganConsult.Document.HttpApi.Host"; Priority=2},
    @{Name="AI"; Port=5008; Path="src/DoganConsult.AI.HttpApi.Host"; Priority=2},
    @{Name="Gateway"; Port=5000; Path="src/gateway/DoganConsult.Gateway"; Priority=3},
    @{Name="Blazor"; Port=5001; Path="src/DoganConsult.Web.Blazor"; Priority=4}
)

# Check if ports are available
function Test-Port {
    param([int]$Port)
    $connection = Test-NetConnection -ComputerName localhost -Port $Port -InformationLevel Quiet -WarningAction SilentlyContinue
    return -not $connection
}

# Start service in background
function Start-Service {
    param($Service)
    
    $port = $Service.Port
    if (-not (Test-Port -Port $port)) {
        Write-Host "  ⚠ Port $port is already in use. Skipping $($Service.Name)" -ForegroundColor Yellow
        return $null
    }
    
    Write-Host "  Starting $($Service.Name) on port $port..." -ForegroundColor Gray
    
    if ($Service.Name -eq "Gateway") {
        $csprojPath = "$($Service.Path)/DoganConsult.Gateway.csproj"
    }
    elseif ($Service.Name -eq "Blazor") {
        $csprojPath = "$($Service.Path)/DoganConsult.Web.Blazor.csproj"
    }
    else {
        $csprojPath = "$($Service.Path)/DoganConsult.$($Service.Name).HttpApi.Host.csproj"
    }
    
    $job = Start-Job -ScriptBlock {
        param($Path, $Port, $Name)
        Set-Location $Path
        $env:ASPNETCORE_URLS = "http://localhost:$Port"
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --no-build
    } -ArgumentList (Resolve-Path $csprojPath).Directory, $port, $Service.Name
    
    Start-Sleep -Seconds 3
    
    if (Test-Port -Port $port) {
        Write-Host "  ✓ $($Service.Name) started successfully" -ForegroundColor Green
        return $job
    } else {
        Write-Host "  ✗ $($Service.Name) failed to start" -ForegroundColor Red
        return $null
    }
}

# Build solution first
Write-Host "[1/3] Building solution..." -ForegroundColor Yellow
Push-Location $PSScriptRoot
dotnet build DoganConsult.Platform.sln -c Release | Out-Null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "  ✓ Build complete" -ForegroundColor Green

# Start services in priority order
Write-Host "`n[2/3] Starting services..." -ForegroundColor Yellow
$jobs = @{}

foreach ($priority in 1..4) {
    $servicesToStart = $services | Where-Object { $_.Priority -eq $priority }
    
    foreach ($service in $servicesToStart) {
        $job = Start-Service -Service $service
        if ($job) {
            $jobs[$service.Name] = $job
        }
        Start-Sleep -Seconds 2
    }
    
    if ($priority -lt 4) {
        Write-Host "  Waiting for services to initialize..." -ForegroundColor Gray
        Start-Sleep -Seconds 5
    }
}

Write-Host "`n[3/3] Service Status:" -ForegroundColor Yellow
foreach ($service in $services) {
    $port = $service.Port
    $url = "http://localhost:$port"
    if (Test-Port -Port $port) {
        Write-Host "  ✓ $($service.Name): $url" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $($service.Name): Not running" -ForegroundColor Red
    }
}

Write-Host "`n=== Services Started ===" -ForegroundColor Green
Write-Host "`nAccess URLs:" -ForegroundColor Cyan
Write-Host "  Blazor UI: http://localhost:5001" -ForegroundColor White
Write-Host "  API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "  Identity: http://localhost:5002" -ForegroundColor White
Write-Host "`nTo stop all services, run: Stop-LocalServices.ps1" -ForegroundColor Yellow
Write-Host "Or press Ctrl+C and run: Get-Job | Stop-Job; Get-Job | Remove-Job" -ForegroundColor Yellow

Pop-Location

# Keep script running
Write-Host "`nPress Ctrl+C to stop all services..." -ForegroundColor Yellow
try {
    while ($true) {
        Start-Sleep -Seconds 10
        # Check if any job failed
        $failedJobs = Get-Job | Where-Object { $_.State -eq "Failed" }
        if ($failedJobs) {
            Write-Host "`n⚠ Some services have failed. Check logs." -ForegroundColor Yellow
        }
    }
}
finally {
    Write-Host "`nStopping all services..." -ForegroundColor Yellow
    Get-Job | Stop-Job
    Get-Job | Remove-Job
    Write-Host "All services stopped." -ForegroundColor Green
}

