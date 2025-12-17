# PowerShell Deployment Script for Hetzner Server
# Server: 46.224.64.95
# Password: aKTbCKAeWapnp9xkLcjF

param(
    [string]$ServerIP = "46.224.64.95",
    [string]$ServerUser = "root",
    [string]$Password = "aKTbCKAeWapnp9xkLcjF"
)

$ErrorActionPreference = "Stop"

Write-Host "=== DoganConsult Platform - Production Deployment ===" -ForegroundColor Cyan
Write-Host "Target: $ServerUser@$ServerIP" -ForegroundColor Yellow
Write-Host ""

# Install plink (PuTTY) if not available
$plinkPath = "C:\Program Files\PuTTY\plink.exe"
if (-not (Test-Path $plinkPath)) {
    Write-Host "PuTTY plink not found. Please install PuTTY from https://www.putty.org/" -ForegroundColor Red
    Write-Host "Or use WSL/Git Bash to run the .sh deployment script" -ForegroundColor Yellow
    exit 1
}

# Create SSH command function
function Invoke-SSHCommand {
    param([string]$Command)
    
    $psi = New-Object System.Diagnostics.ProcessStartInfo
    $psi.FileName = $plinkPath
    $psi.Arguments = "-batch -pw `"$Password`" $ServerUser@$ServerIP `"$Command`""
    $psi.UseShellExecute = $false
    $psi.RedirectStandardOutput = $true
    $psi.RedirectStandardError = $true
    
    $process = New-Object System.Diagnostics.Process
    $process.StartInfo = $psi
    $process.Start() | Out-Null
    
    $output = $process.StandardOutput.ReadToEnd()
    $error = $process.StandardError.ReadToEnd()
    $process.WaitForExit()
    
    if ($process.ExitCode -ne 0) {
        Write-Host "Error: $error" -ForegroundColor Red
        throw "Command failed: $Command"
    }
    
    return $output
}

Write-Host "Step 1: Connecting to server..." -ForegroundColor Green
try {
    $result = Invoke-SSHCommand "echo 'Connection successful' && uname -a"
    Write-Host $result -ForegroundColor Gray
} catch {
    Write-Host "Failed to connect to server. Please check credentials." -ForegroundColor Red
    exit 1
}

Write-Host "`nStep 2: Installing Docker and Docker Compose..." -ForegroundColor Green
$dockerInstall = @"
    # Update system
    apt-get update
    
    # Install prerequisites
    apt-get install -y ca-certificates curl gnupg lsb-release
    
    # Add Docker's GPG key
    install -m 0755 -d /etc/apt/keyrings
    curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
    chmod a+r /etc/apt/keyrings/docker.asc
    
    # Add Docker repository
    echo "deb [arch=\$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \$(. /etc/os-release && echo \"\$VERSION_CODENAME\") stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null
    
    # Install Docker
    apt-get update
    apt-get install -y docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin
    
    # Start Docker
    systemctl start docker
    systemctl enable docker
    
    echo 'Docker installed successfully'
"@

try {
    $result = Invoke-SSHCommand $dockerInstall
    Write-Host $result -ForegroundColor Gray
} catch {
    Write-Host "Warning: Docker installation may have failed. Continuing..." -ForegroundColor Yellow
}

Write-Host "`nStep 3: Creating deployment directory..." -ForegroundColor Green
$createDirs = @"
    mkdir -p /opt/doganconsult
    cd /opt/doganconsult
    echo 'Directory created'
"@

Invoke-SSHCommand $createDirs | Write-Host -ForegroundColor Gray

Write-Host "`nStep 4: Transferring docker-compose.yml..." -ForegroundColor Green
# Use pscp (PuTTY SCP) to transfer files
$pscpPath = "C:\Program Files\PuTTY\pscp.exe"
if (Test-Path $pscpPath) {
    $dockerComposePath = ".\docker-compose.yml"
    & $pscpPath -batch -pw $Password $dockerComposePath "${ServerUser}@${ServerIP}:/opt/doganconsult/docker-compose.yml"
} else {
    Write-Host "pscp not found. Please manually copy docker-compose.yml to server" -ForegroundColor Yellow
}

Write-Host "`nStep 5: Starting services with Docker Compose..." -ForegroundColor Green
$startServices = @"
    cd /opt/doganconsult
    docker compose pull
    docker compose up -d
    docker compose ps
"@

try {
    $result = Invoke-SSHCommand $startServices
    Write-Host $result -ForegroundColor Gray
} catch {
    Write-Host "Failed to start services. You may need to configure environment variables." -ForegroundColor Yellow
}

Write-Host "`n=== Deployment Summary ===" -ForegroundColor Cyan
Write-Host "Server IP: $ServerIP" -ForegroundColor White
Write-Host "Services should be accessible at:" -ForegroundColor White
Write-Host "  - API Gateway: http://${ServerIP}:5000" -ForegroundColor Yellow
Write-Host "  - Blazor UI: http://${ServerIP}:5001" -ForegroundColor Yellow
Write-Host "  - Identity Service: http://${ServerIP}:5002" -ForegroundColor Yellow
Write-Host ""
Write-Host "To check logs: ssh $ServerUser@$ServerIP 'cd /opt/doganconsult && docker compose logs -f'" -ForegroundColor Gray
Write-Host ""
