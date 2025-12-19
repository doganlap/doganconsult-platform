# Stop all existing dotnet processes
Get-Process dotnet -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2

# Define services
$services = @(
    @{Name="Identity"; Path="aspnet-core/src/DoganConsult.Identity.HttpApi.Host"; Port=44346},
    @{Name="Organization"; Path="aspnet-core/src/DoganConsult.Organization.HttpApi.Host"; Port=44337},
    @{Name="Workspace"; Path="aspnet-core/src/DoganConsult.Workspace.HttpApi.Host"; Port=44371},
    @{Name="Document"; Path="aspnet-core/src/DoganConsult.Document.HttpApi.Host"; Port=44348},
    @{Name="UserProfile"; Path="aspnet-core/src/DoganConsult.UserProfile.HttpApi.Host"; Port=44327},
    @{Name="AI"; Path="aspnet-core/src/DoganConsult.AI.HttpApi.Host"; Port=44331},
    @{Name="Audit"; Path="aspnet-core/src/DoganConsult.Audit.HttpApi.Host"; Port=44375},
    @{Name="Gateway"; Path="aspnet-core/src/gateway/DoganConsult.Gateway"; Port=5000},
    @{Name="Blazor"; Path="aspnet-core/src/DoganConsult.Web.Blazor"; Port=44373}
)

# Start each service in background
foreach ($service in $services) {
    Write-Host "Starting $($service.Name) service..."
    $servicePath = Join-Path "d:\test" $service.Path

    if (Test-Path $servicePath) {
        Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$servicePath'; dotnet run --configuration Debug --no-build"
        Start-Sleep -Seconds 5 # Increased sleep to allow services to initialize
    } else {
        Write-Host "  ERROR: Path not found: $servicePath"
    }
}

Write-Host "All services started!"
Write-Host "Identity: https://localhost:44346"
Write-Host "Organization: https://localhost:44337"
Write-Host "Workspace: https://localhost:44371"
Write-Host "Document: https://localhost:44348"
Write-Host "UserProfile: https://localhost:44327"
Write-Host "AI: https://localhost:44331"
Write-Host "Audit: https://localhost:44375"
Write-Host "Gateway: http://localhost:5000"
Write-Host "Blazor UI: https://localhost:44373"
