# Complete Production Deployment Script for Hetzner2
# Server: 46.4.206.15

param(
    [string]$ServerIP = "46.4.206.15",
    [string]$ServerUser = "root",
    [string]$DeployPath = "/opt/doganconsult"
)

$ErrorActionPreference = "Stop"

Write-Host "=== DoganConsult Platform - Production Deployment ===" -ForegroundColor Green
Write-Host "Server: $ServerUser@$ServerIP" -ForegroundColor Cyan
Write-Host "Deploy Path: $DeployPath" -ForegroundColor Cyan
Write-Host ""

# Database connection strings (Production Railway)
$dbConnections = @{
    Identity = "Host=nozomi.proxy.rlwy.net;Port=35537;Database=railway;Username=postgres;Password=mcmfdSTZUcDwqJtwkQXzJQSTurwVaQvz;SSL Mode=Require;"
    Organization = "Host=metro.proxy.rlwy.net;Port=47319;Database=railway;Username=postgres;Password=dEUIWJcxwajoHIoLwqTnAITbgXraTTKc;SSL Mode=Require;"
    Workspace = "Host=switchyard.proxy.rlwy.net;Port=37561;Database=railway;Username=postgres;Password=EwciEISIVdnEulryLkVgqHOVlaqjiyML;SSL Mode=Require;"
    UserProfile = "Host=hopper.proxy.rlwy.net;Port=47669;Database=railway;Username=postgres;Password=lKZKCQxdrBpklnirrzTSzznhzofmdNlv;SSL Mode=Require;"
    Audit = "Host=crossover.proxy.rlwy.net;Port=17109;Database=railway;Username=postgres;Password=PxccVRRaJCXlJGVdrzDTCGfApuJQEFVo;SSL Mode=Require;"
    Document = "Host=yamanote.proxy.rlwy.net;Port=35357;Database=railway;Username=postgres;Password=sCjZiToIMDSAznQNZZXcmAJlnLcElioE;SSL Mode=Require;"
    AI = "Host=ballast.proxy.rlwy.net;Port=53629;Database=railway;Username=postgres;Password=RRcrRrKgksUqapCckJJqBUIyWBhoNDJg;SSL Mode=Require;"
}

# Redis configuration
$redisConfig = "interchange.proxy.rlwy.net:26424,password=sOJrVPlSFlDQQpMizveGoYpFyzuNiPIv,ssl=false,abortConnect=false"

# Services configuration
$services = @(
    @{Name="Identity"; Port=5002; Path="src/DoganConsult.Identity.HttpApi.Host"; DbKey="Identity"; IsApiHost=$true},
    @{Name="Organization"; Port=5003; Path="src/DoganConsult.Organization.HttpApi.Host"; DbKey="Organization"; IsApiHost=$true},
    @{Name="Workspace"; Port=5004; Path="src/DoganConsult.Workspace.HttpApi.Host"; DbKey="Workspace"; IsApiHost=$true},
    @{Name="UserProfile"; Port=5005; Path="src/DoganConsult.UserProfile.HttpApi.Host"; DbKey="UserProfile"; IsApiHost=$true},
    @{Name="Audit"; Port=5006; Path="src/DoganConsult.Audit.HttpApi.Host"; DbKey="Audit"; IsApiHost=$true},
    @{Name="Document"; Port=5007; Path="src/DoganConsult.Document.HttpApi.Host"; DbKey="Document"; IsApiHost=$true},
    @{Name="AI"; Port=5008; Path="src/DoganConsult.AI.HttpApi.Host"; DbKey="AI"; IsApiHost=$true},
    @{Name="Gateway"; Port=5000; Path="src/gateway/DoganConsult.Gateway"; DbKey=$null; IsApiHost=$false},
    @{Name="Blazor"; Port=5001; Path="src/DoganConsult.Web.Blazor"; DbKey=$null; IsApiHost=$false}
)

# Step 1: Build solution
Write-Host "`n[1/7] Building solution..." -ForegroundColor Yellow
Push-Location $PSScriptRoot
dotnet build DoganConsult.Platform.sln -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    exit 1
}

# Build HttpApi.Client projects in Release (they're often Debug by default)
Write-Host "  Building HttpApi.Client projects in Release mode..." -ForegroundColor Gray
Get-ChildItem -Path src -Filter "*HttpApi.Client.csproj" -Recurse | ForEach-Object {
    Write-Host "    Building $($_.Name)..." -ForegroundColor DarkGray
    dotnet build $_.FullName -c Release --no-restore
    if ($LASTEXITCODE -ne 0) {
        Write-Host "    Failed to build $($_.Name), retrying with restore..." -ForegroundColor Yellow
        dotnet build $_.FullName -c Release
    }
}

# Step 2: Create production appsettings
Write-Host "`n[2/7] Creating production appsettings..." -ForegroundColor Yellow
$identityService = $services | Where-Object { $_.Name -eq "Identity" } | Select-Object -First 1
$authUrl = "http://${ServerIP}:$($identityService.Port)"
$gatewayUrl = "http://${ServerIP}:5000"

foreach ($service in $services) {
    if ($service.IsApiHost -and $service.DbKey) {
        # API Host services with database
        $appsettingsPath = "$($service.Path)/appsettings.Production.json"
        $selfUrl = "http://${ServerIP}:$($service.Port)"
        
        $appsettings = @{
            App = @{
                SelfUrl = $selfUrl
            }
            ConnectionStrings = @{
                Default = $dbConnections[$service.DbKey]
            }
            AuthServer = @{
                Authority = $authUrl
                RequireHttpsMetadata = $false
            }
            RemoteServices = @{
                Default = @{
                    BaseUrl = $gatewayUrl
                }
            }
        }
        
        if ($service.Name -eq "AI") {
            $appsettings["Redis"] = @{
                Configuration = $redisConfig
                InstanceName = "DoganConsult:"
            }
        }
        
        $appsettings | ConvertTo-Json -Depth 10 | Out-File -FilePath $appsettingsPath -Encoding UTF8
        Write-Host "  Created: $appsettingsPath" -ForegroundColor Gray
    }
    elseif ($service.Name -eq "Gateway") {
        # Gateway service
        $appsettingsPath = "$($service.Path)/appsettings.Production.json"
        $selfUrl = "http://${ServerIP}:$($service.Port)"
        
        $appsettings = @{
            App = @{
                SelfUrl = $selfUrl
            }
        }
        
        $appsettings | ConvertTo-Json -Depth 10 | Out-File -FilePath $appsettingsPath -Encoding UTF8
        Write-Host "  Created: $appsettingsPath" -ForegroundColor Gray
    }
    elseif ($service.Name -eq "Blazor") {
        # Blazor UI service
        $appsettingsPath = "$($service.Path)/appsettings.Production.json"
        $selfUrl = "http://${ServerIP}:$($service.Port)"
        
        $appsettings = @{
            App = @{
                SelfUrl = $selfUrl
            }
            AuthServer = @{
                Authority = $authUrl
                RequireHttpsMetadata = $false
            }
            RemoteServices = @{
                Default = @{
                    BaseUrl = $gatewayUrl
                }
            }
        }
        
        $appsettings | ConvertTo-Json -Depth 10 | Out-File -FilePath $appsettingsPath -Encoding UTF8
        Write-Host "  Created: $appsettingsPath" -ForegroundColor Gray
    }
}

# Step 3: Publish all services
Write-Host "`n[3/7] Publishing services..." -ForegroundColor Yellow
if (Test-Path "publish") {
    Remove-Item -Recurse -Force "publish"
}
New-Item -ItemType Directory -Path "publish" | Out-Null

foreach ($service in $services) {
    if (Test-Path $service.Path) {
        Write-Host "  Publishing $($service.Name)..." -ForegroundColor Gray
        $publishPath = "publish/$($service.Name)"
        
        if ($service.Name -eq "Gateway") {
            $csprojPath = "$($service.Path)/DoganConsult.Gateway.csproj"
        }
        elseif ($service.Name -eq "Blazor") {
            $csprojPath = "$($service.Path)/DoganConsult.Web.Blazor.csproj"
            # Publish Blazor with workaround for Gateway appsettings conflict
            # Temporarily rename Gateway appsettings files
            $gatewayPath = "src/gateway/DoganConsult.Gateway"
            $gatewayAppSettings = @(
                "$gatewayPath/appsettings.json",
                "$gatewayPath/appsettings.Development.json",
                "$gatewayPath/appsettings.Production.json"
            )
            $renamedFiles = @()
            foreach ($file in $gatewayAppSettings) {
                if (Test-Path $file) {
                    $dir = Split-Path -Parent $file
                    $name = Split-Path -Leaf $file
                    $backupPath = Join-Path $dir "$name.backup_deploy"
                    try {
                        Copy-Item -Path $file -Destination $backupPath -Force
                        Remove-Item -Path $file -Force
                        $renamedFiles += @{Original=$file; Backup=$backupPath}
                    } catch {
                        Write-Host "    Warning: Could not backup $file" -ForegroundColor Yellow
                    }
                }
            }
            try {
                # First try without --no-build, which ensures all dependencies are properly built
                dotnet publish $csprojPath -c Release -o $publishPath
            }
            finally {
                # Restore renamed files
                foreach ($fileInfo in $renamedFiles) {
                    if (Test-Path $fileInfo.Backup) {
                        try {
                            Copy-Item -Path $fileInfo.Backup -Destination $fileInfo.Original -Force
                            Remove-Item -Path $fileInfo.Backup -Force
                        } catch {
                            Write-Host "    Warning: Could not restore $($fileInfo.Original)" -ForegroundColor Yellow
                        }
                    }
                }
            }
        }
        else {
            $csprojPath = "$($service.Path)/DoganConsult.$($service.Name).HttpApi.Host.csproj"
            dotnet publish $csprojPath -c Release -o $publishPath --no-build
        }
        
        if ($service.Name -ne "Blazor" -and $service.Name -ne "Gateway") {
            # Already published above for non-Blazor, non-Gateway services
        }
        if ($LASTEXITCODE -ne 0) {
            Write-Host "Failed to publish $($service.Name)" -ForegroundColor Red
            exit 1
        }
    }
}

# Step 4: Create deployment package
Write-Host "`n[4/7] Creating deployment package..." -ForegroundColor Yellow
$deployScript = @"
#!/bin/bash
set -e

DEPLOY_PATH="$DeployPath"
SERVER_IP="$ServerIP"

echo "=== Setting up DoganConsult Platform on Hetzner2 ==="

# Create user
if ! id "doganconsult" &>/dev/null; then
    useradd -r -s /bin/bash -d `$DEPLOY_PATH doganconsult
    echo "Created user: doganconsult"
fi

# Create directories
mkdir -p `$DEPLOY_PATH/{services,logs,config,ssl}
chown -R doganconsult:doganconsult `$DEPLOY_PATH

# Install .NET 10.0 Runtime
if ! command -v dotnet &> /dev/null; then
    echo "Installing .NET 10.0 Runtime..."
    wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh
    chmod +x /tmp/dotnet-install.sh
    /tmp/dotnet-install.sh --channel 10.0 --runtime aspnetcore --install-dir /usr/share/dotnet
    ln -sf /usr/share/dotnet/dotnet /usr/bin/dotnet
    echo "export PATH=`$PATH:/usr/share/dotnet" >> /etc/profile
fi

# Copy published files
echo "Copying application files..."
cp -r publish/* `$DEPLOY_PATH/services/

# Set permissions
chown -R doganconsult:doganconsult `$DEPLOY_PATH
chmod +x `$DEPLOY_PATH/services/*/*.dll

echo "Setup complete!"
"@

$deployScript | Out-File -FilePath "publish/setup-server.sh" -Encoding UTF8 -NoNewline
Write-Host "  Created setup script" -ForegroundColor Gray

# Step 5: Copy to server
Write-Host "`n[5/7] Copying files to server..." -ForegroundColor Yellow
Write-Host "  Creating tarball..." -ForegroundColor Gray
tar -czf deploy.tar.gz publish/ 2>$null
if ($LASTEXITCODE -ne 0) {
    # Use PowerShell compression if tar not available
    Compress-Archive -Path "publish/*" -DestinationPath "deploy.zip" -Force
    Write-Host "  Uploading deploy.zip..." -ForegroundColor Gray
    scp deploy.zip "${ServerUser}@${ServerIP}:/tmp/"
    $useZip = $true
} else {
    Write-Host "  Uploading deploy.tar.gz..." -ForegroundColor Gray
    scp deploy.tar.gz "${ServerUser}@${ServerIP}:/tmp/"
    $useZip = $false
}

# Step 6: Setup on server
Write-Host "`n[6/7] Setting up server..." -ForegroundColor Yellow
$sshCommands = @"
set -e
cd /tmp
if [ -f deploy.tar.gz ]; then
    tar -xzf deploy.tar.gz
elif [ -f deploy.zip ]; then
    unzip -q deploy.zip
fi
chmod +x publish/setup-server.sh
bash publish/setup-server.sh
"@

$sshCommands | ssh "${ServerUser}@${ServerIP}" bash
if ($LASTEXITCODE -ne 0) {
    Write-Host "Server setup failed!" -ForegroundColor Red
    exit 1
}

# Step 7: Create and install systemd services
Write-Host "`n[7/7] Creating systemd services..." -ForegroundColor Yellow
$systemdScript = @"
#!/bin/bash
set -e

DEPLOY_PATH="$DeployPath"
SERVICES_PATH="`$DEPLOY_PATH/services"

# Create systemd service files
create_service() {
    local service_name=`$1
    local service_dir=`$2
    local port=`$3
    local dll_name="DoganConsult.`$service_dir.HttpApi.Host.dll"
    
    cat > /tmp/doganconsult-`$service_name.service <<EOF
[Unit]
Description=Dogan Consult `$service_dir Service
After=network.target

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=`$SERVICES_PATH/`$service_dir
ExecStart=/usr/bin/dotnet `$SERVICES_PATH/`$service_dir/`$dll_name
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-`$service_name
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:`$port

[Install]
WantedBy=multi-user.target
EOF
}

create_service "identity" "Identity" 5002
create_service "organization" "Organization" 5003
create_service "workspace" "Workspace" 5004
create_service "userprofile" "UserProfile" 5005
create_service "audit" "Audit" 5006
create_service "document" "Document" 5007
create_service "ai" "AI" 5008

# Gateway service (different DLL name)
cat > /tmp/doganconsult-gateway.service <<EOF
[Unit]
Description=Dogan Consult Gateway Service
After=network.target
After=doganconsult-identity.service

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=`$SERVICES_PATH/Gateway
ExecStart=/usr/bin/dotnet `$SERVICES_PATH/Gateway/DoganConsult.Gateway.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-gateway
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5000

[Install]
WantedBy=multi-user.target
EOF

# Blazor UI service (different DLL name)
cat > /tmp/doganconsult-blazor.service <<EOF
[Unit]
Description=Dogan Consult Blazor UI Service
After=network.target
After=doganconsult-gateway.service

[Service]
Type=notify
User=doganconsult
Group=doganconsult
WorkingDirectory=`$SERVICES_PATH/Blazor
ExecStart=/usr/bin/dotnet `$SERVICES_PATH/Blazor/DoganConsult.Web.Blazor.dll
Restart=always
RestartSec=10
StandardOutput=journal
StandardError=journal
SyslogIdentifier=doganconsult-blazor
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://0.0.0.0:5001

[Install]
WantedBy=multi-user.target
EOF

# Install services
cp /tmp/doganconsult-*.service /etc/systemd/system/
systemctl daemon-reload

echo "Systemd services created!"
echo "To start: systemctl start doganconsult-*"
echo "To enable: systemctl enable doganconsult-*"
"@

$systemdScript | Out-File -FilePath "publish/create-systemd.sh" -Encoding UTF8 -NoNewline
scp publish/create-systemd.sh "${ServerUser}@${ServerIP}:/tmp/"
ssh "${ServerUser}@${ServerIP}" "chmod +x /tmp/create-systemd.sh && bash /tmp/create-systemd.sh"

Write-Host "`n=== Deployment Complete ===" -ForegroundColor Green
Write-Host "`nNext steps:" -ForegroundColor Yellow
Write-Host "1. SSH to server: ssh $ServerUser@$ServerIP" -ForegroundColor Cyan
Write-Host "2. Run migrations: cd $DeployPath/services/Identity && dotnet DoganConsult.Identity.DbMigrator.dll" -ForegroundColor Cyan
Write-Host "3. Start services: systemctl start doganconsult-*" -ForegroundColor Cyan
Write-Host "4. Enable on boot: systemctl enable doganconsult-*" -ForegroundColor Cyan
Write-Host "5. Check status: systemctl status doganconsult-identity" -ForegroundColor Cyan

Pop-Location
