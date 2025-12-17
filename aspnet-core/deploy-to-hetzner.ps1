# PowerShell script to deploy all services to Hetzner server (without Docker)

param(
    [Parameter(Mandatory=$true)]
    [string]$ServerIP = "46.224.64.95",
    
    [Parameter(Mandatory=$true)]
    [string]$ServerUser = "root",
    
    [Parameter(Mandatory=$false)]
    [string]$DeployPath = "/opt/doganconsult"
)

Write-Host "=== Deploying to Hetzner Server ===" -ForegroundColor Green
Write-Host "Server: $ServerUser@$ServerIP" -ForegroundColor Cyan
Write-Host "Deploy Path: $DeployPath" -ForegroundColor Cyan
Write-Host ""

# Services to deploy
$services = @(
    @{Name="Identity"; Port=5002; Path="src/DoganConsult.Identity.HttpApi.Host"},
    @{Name="Organization"; Port=5003; Path="src/DoganConsult.Organization.HttpApi.Host"},
    @{Name="Workspace"; Port=5004; Path="src/DoganConsult.Workspace.HttpApi.Host"},
    @{Name="UserProfile"; Port=5005; Path="src/DoganConsult.UserProfile.HttpApi.Host"},
    @{Name="Audit"; Port=5006; Path="src/DoganConsult.Audit.HttpApi.Host"},
    @{Name="Document"; Port=5007; Path="src/DoganConsult.Document.HttpApi.Host"},
    @{Name="AI"; Port=5008; Path="src/DoganConsult.AI.HttpApi.Host"},
    @{Name="Gateway"; Port=5000; Path="src/gateway/DoganConsult.Gateway"},
    @{Name="Blazor"; Port=5001; Path="src/DoganConsult.Web.Blazor"}
)

# Build all projects
Write-Host "Building all projects..." -ForegroundColor Yellow
dotnet build DoganConsult.Platform.sln -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Publish each service
foreach ($service in $services) {
    Write-Host "Publishing $($service.Name)..." -ForegroundColor Yellow
    $publishPath = "publish/$($service.Name)"
    
    if (Test-Path $service.Path) {
        dotnet publish $service.Path -c Release -o $publishPath --no-build
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to publish $($service.Name)" -ForegroundColor Red
            exit 1
        }
    }
}

Write-Host "`n=== Publishing Complete ===" -ForegroundColor Green
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Copy publish folder to server: scp -r publish $ServerUser@$ServerIP:$DeployPath/" -ForegroundColor Cyan
Write-Host "2. SSH to server and run: ssh $ServerUser@$ServerIP" -ForegroundColor Cyan
Write-Host "3. Run setup script on server: bash $DeployPath/setup-services.sh" -ForegroundColor Cyan
