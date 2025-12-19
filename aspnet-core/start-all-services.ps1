# DoganConsult Platform - Start All Services
# This script starts all 9 services from source with proper ordering and health checks

$ErrorActionPreference = "Stop"

Write-Host "`n=== DoganConsult Platform - Starting All Services ===" -ForegroundColor Green
Write-Host "Time: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')" -ForegroundColor Gray
Write-Host ""

# Service configuration with proper startup order
$services = @(
    @{Name="Identity"; Port=5002; Path="src/DoganConsult.Identity.HttpApi.Host"; Project="DoganConsult.Identity.HttpApi.Host.csproj"; Priority=1},
    @{Name="Organization"; Port=5003; Path="src/DoganConsult.Organization.HttpApi.Host"; Project="DoganConsult.Organization.HttpApi.Host.csproj"; Priority=2},
    @{Name="Workspace"; Port=5004; Path="src/DoganConsult.Workspace.HttpApi.Host"; Project="DoganConsult.Workspace.HttpApi.Host.csproj"; Priority=2},
    @{Name="UserProfile"; Port=5005; Path="src/DoganConsult.UserProfile.HttpApi.Host"; Project="DoganConsult.UserProfile.HttpApi.Host.csproj"; Priority=2},
    @{Name="Audit"; Port=5006; Path="src/DoganConsult.Audit.HttpApi.Host"; Project="DoganConsult.Audit.HttpApi.Host.csproj"; Priority=2},
    @{Name="Document"; Port=5007; Path="src/DoganConsult.Document.HttpApi.Host"; Project="DoganConsult.Document.HttpApi.Host.csproj"; Priority=2},
    @{Name="AI"; Port=5008; Path="src/DoganConsult.AI.HttpApi.Host"; Project="DoganConsult.AI.HttpApi.Host.csproj"; Priority=2},
    @{Name="Gateway"; Port=5000; Path="src/gateway/DoganConsult.Gateway"; Project="DoganConsult.Gateway.csproj"; Priority=3},
    @{Name="Blazor"; Port=5001; Path="src/DoganConsult.Web.Blazor"; Project="DoganConsult.Web.Blazor.csproj"; Priority=4}
)

# Step 1: Check for running services
Write-Host "[Step 1/5] Checking for existing services..." -ForegroundColor Yellow
$existingProcesses = Get-Process -Name "dotnet" -ErrorAction SilentlyContinue
if ($existingProcesses) {
    Write-Host "  Found $($existingProcesses.Count) existing dotnet process(es)" -ForegroundColor Yellow
    Write-Host "  Stopping existing services..." -ForegroundColor Gray
    $existingProcesses | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 3
    Write-Host "  ✓ Existing services stopped" -ForegroundColor Green
} else {
    Write-Host "  ✓ No existing services found" -ForegroundColor Green
}

# Step 2: Build solution
Write-Host "`n[Step 2/5] Building solution..." -ForegroundColor Yellow
Push-Location $PSScriptRoot

$buildOutput = dotnet build DoganConsult.Platform.sln -c Release 2>&1
$buildSuccess = $LASTEXITCODE -eq 0

if ($buildSuccess) {
    Write-Host "  ✓ Build completed successfully" -ForegroundColor Green
} else {
    Write-Host "  ✗ Build failed!" -ForegroundColor Red
    Write-Host "`nBuild output (last 20 lines):" -ForegroundColor Yellow
    $buildOutput | Select-Object -Last 20 | ForEach-Object { Write-Host "    $_" }
    Pop-Location
    exit 1
}

# Step 3: Start services in priority order
Write-Host "`n[Step 3/5] Starting services..." -ForegroundColor Yellow

# Priority 1: Identity (must start first)
$identity = $services | Where-Object { $_.Priority -eq 1 } | Select-Object -First 1
Write-Host "  Starting $($identity.Name) service (port $($identity.Port))..." -ForegroundColor Cyan
$identityPath = Join-Path $PSScriptRoot $identity.Path
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$identityPath'; `$env:ASPNETCORE_URLS='http://localhost:$($identity.Port)'; `$env:ASPNETCORE_ENVIRONMENT='Development'; Write-Host 'Starting $($identity.Name) Service...' -ForegroundColor Green; dotnet run --no-build" -WindowStyle Normal
Start-Sleep -Seconds 10
Write-Host "  ✓ Identity service started" -ForegroundColor Green

# Priority 2: Microservices (can start in parallel)
$microservices = $services | Where-Object { $_.Priority -eq 2 }
foreach ($service in $microservices) {
    Write-Host "  Starting $($service.Name) service (port $($service.Port))..." -ForegroundColor Cyan
    $servicePath = Join-Path $PSScriptRoot $service.Path
    Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$servicePath'; `$env:ASPNETCORE_URLS='http://localhost:$($service.Port)'; `$env:ASPNETCORE_ENVIRONMENT='Development'; Write-Host 'Starting $($service.Name) Service...' -ForegroundColor Green; dotnet run --no-build" -WindowStyle Normal
    Start-Sleep -Seconds 2
}
Write-Host "  ✓ All microservices started" -ForegroundColor Green
Start-Sleep -Seconds 8

# Priority 3: Gateway
$gateway = $services | Where-Object { $_.Priority -eq 3 } | Select-Object -First 1
Write-Host "  Starting $($gateway.Name) (port $($gateway.Port))..." -ForegroundColor Cyan
$gatewayPath = Join-Path $PSScriptRoot $gateway.Path
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$gatewayPath'; `$env:ASPNETCORE_URLS='http://localhost:$($gateway.Port)'; `$env:ASPNETCORE_ENVIRONMENT='Development'; Write-Host 'Starting Gateway...' -ForegroundColor Green; dotnet run --no-build" -WindowStyle Normal
Start-Sleep -Seconds 5
Write-Host "  ✓ Gateway started" -ForegroundColor Green

# Priority 4: Blazor UI
$blazor = $services | Where-Object { $_.Priority -eq 4 } | Select-Object -First 1
Write-Host "  Starting $($blazor.Name) UI (port $($blazor.Port))..." -ForegroundColor Cyan
$blazorPath = Join-Path $PSScriptRoot $blazor.Path
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$blazorPath'; `$env:ASPNETCORE_URLS='http://localhost:$($blazor.Port)'; `$env:ASPNETCORE_ENVIRONMENT='Development'; Write-Host 'Starting Blazor UI...' -ForegroundColor Green; dotnet run --no-build" -WindowStyle Normal
Write-Host "  ✓ Blazor UI started" -ForegroundColor Green

# Step 4: Wait for services to initialize
Write-Host "`n[Step 4/5] Waiting for services to initialize..." -ForegroundColor Yellow
Write-Host "  This will take about 30 seconds..." -ForegroundColor Gray
Start-Sleep -Seconds 30

# Step 5: Health check
Write-Host "`n[Step 5/5] Performing health check..." -ForegroundColor Yellow
$allHealthy = $true
foreach ($service in $services) {
    $port = $service.Port
    $conn = Test-NetConnection -ComputerName localhost -Port $port -InformationLevel Quiet -WarningAction SilentlyContinue -ErrorAction SilentlyContinue
    if ($conn) {
        Write-Host "  ✓ $($service.Name) (port $port) - RUNNING" -ForegroundColor Green
    } else {
        Write-Host "  ✗ $($service.Name) (port $port) - NOT RUNNING" -ForegroundColor Red
        $allHealthy = $false
    }
}

Pop-Location

# Final status
Write-Host "`n========================================" -ForegroundColor Cyan
if ($allHealthy) {
    Write-Host "🎉 ALL SERVICES ARE RUNNING!" -ForegroundColor Green
    Write-Host "`n📱 Access your application:" -ForegroundColor Cyan
    Write-Host "   🌐 Blazor UI:  http://localhost:5001" -ForegroundColor White
    Write-Host "   🔧 Gateway:    http://localhost:5000" -ForegroundColor White
    Write-Host "   🔐 Identity:   http://localhost:5002" -ForegroundColor White
    Write-Host "`n💡 Opening browser..." -ForegroundColor Yellow
    Start-Process "http://localhost:5001"
    Write-Host "`n✅ Platform is ready to use!" -ForegroundColor Green
} else {
    Write-Host "⚠ SOME SERVICES FAILED TO START" -ForegroundColor Yellow
    Write-Host "`n📋 Troubleshooting:" -ForegroundColor Cyan
    Write-Host "   1. Check the PowerShell windows for error messages" -ForegroundColor White
    Write-Host "   2. Run: .\check-services.ps1" -ForegroundColor White
    Write-Host "   3. Check database connections in appsettings.Development.json" -ForegroundColor White
}

Write-Host "`n🛑 To stop all services, run: .\stop-all-services.ps1" -ForegroundColor Gray
Write-Host "========================================`n" -ForegroundColor Cyan

