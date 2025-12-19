# Quick Platform Start Script
$ErrorActionPreference = "Continue"

Write-Host "=== Starting DoganConsult Platform ===" -ForegroundColor Green
Write-Host ""

Push-Location $PSScriptRoot

# Build if needed
if (-not (Test-Path "src/DoganConsult.Identity.HttpApi.Host/bin/Release")) {
    Write-Host "Building solution..." -ForegroundColor Yellow
    dotnet build DoganConsult.Platform.sln -c Release | Out-Null
}

# Start services in separate windows
Write-Host "Starting services..." -ForegroundColor Yellow

$services = @(
    @{Name="Identity"; Port=5002; Path="src/DoganConsult.Identity.HttpApi.Host"; DLL="DoganConsult.Identity.HttpApi.Host.dll"},
    @{Name="Organization"; Port=5003; Path="src/DoganConsult.Organization.HttpApi.Host"; DLL="DoganConsult.Organization.HttpApi.Host.dll"},
    @{Name="Workspace"; Port=5004; Path="src/DoganConsult.Workspace.HttpApi.Host"; DLL="DoganConsult.Workspace.HttpApi.Host.dll"},
    @{Name="UserProfile"; Port=5005; Path="src/DoganConsult.UserProfile.HttpApi.Host"; DLL="DoganConsult.UserProfile.HttpApi.Host.dll"},
    @{Name="Audit"; Port=5006; Path="src/DoganConsult.Audit.HttpApi.Host"; DLL="DoganConsult.Audit.HttpApi.Host.dll"},
    @{Name="Document"; Port=5007; Path="src/DoganConsult.Document.HttpApi.Host"; DLL="DoganConsult.Document.HttpApi.Host.dll"},
    @{Name="AI"; Port=5008; Path="src/DoganConsult.AI.HttpApi.Host"; DLL="DoganConsult.AI.HttpApi.Host.dll"},
    @{Name="Gateway"; Port=5000; Path="src/gateway/DoganConsult.Gateway"; DLL="DoganConsult.Gateway.dll"},
    @{Name="Blazor"; Port=5001; Path="src/DoganConsult.Web.Blazor"; DLL="DoganConsult.Web.Blazor.dll"}
)

# Start Identity first
Write-Host "Starting Identity service (port 5002)..." -ForegroundColor Cyan
$identity = $services[0]
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\$($identity.Path)'; `$env:ASPNETCORE_URLS='http://localhost:$($identity.Port)'; dotnet run --no-build" -WindowStyle Minimized
Start-Sleep -Seconds 5

# Start other microservices
foreach ($service in $services[1..6]) {
    Write-Host "Starting $($service.Name) service (port $($service.Port))..." -ForegroundColor Cyan
    Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\$($service.Path)'; `$env:ASPNETCORE_URLS='http://localhost:$($service.Port)'; dotnet run --no-build" -WindowStyle Minimized
    Start-Sleep -Seconds 2
}

# Start Gateway
Write-Host "Starting Gateway (port 5000)..." -ForegroundColor Cyan
$gateway = $services[7]
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\$($gateway.Path)'; `$env:ASPNETCORE_URLS='http://localhost:$($gateway.Port)'; dotnet run --no-build" -WindowStyle Minimized
Start-Sleep -Seconds 3

# Start Blazor UI
Write-Host "Starting Blazor UI (port 5001)..." -ForegroundColor Cyan
$blazor = $services[8]
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot\$($blazor.Path)'; `$env:ASPNETCORE_URLS='http://localhost:$($blazor.Port)'; dotnet run --no-build" -WindowStyle Minimized

Write-Host "`n=== Services Starting ===" -ForegroundColor Green
Write-Host "`nAccess URLs:" -ForegroundColor Cyan
Write-Host "  🌐 Blazor UI: http://localhost:5001" -ForegroundColor White
Write-Host "  🔌 API Gateway: http://localhost:5000" -ForegroundColor White
Write-Host "  🔐 Identity: http://localhost:5002" -ForegroundColor White
Write-Host "`nServices are starting in separate windows..." -ForegroundColor Yellow
Write-Host "Wait 30-60 seconds for all services to initialize." -ForegroundColor Yellow
Write-Host "`nTo stop: Close the PowerShell windows or run .\stop-platform.ps1" -ForegroundColor Gray

Pop-Location

