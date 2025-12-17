# PowerShell automated deployment to Hetzner server
# Run as: powershell -ExecutionPolicy Bypass -File deploy-now.ps1

$SERVER_IP = "46.224.64.95"
$SERVER_USER = "root"
$SERVER_PASS = "aKTbCKAeWapnp9xkLcjF"

Write-Host "=== DoganConsult Platform - Automated Deployment ===" -ForegroundColor Cyan
Write-Host "Target: $SERVER_USER@$SERVER_IP" -ForegroundColor Yellow
Write-Host ""

# Function to run SSH commands
function Invoke-SSHCommand {
    param(
        [string]$Command,
        [string]$Description = ""
    )
    
    if ($Description) {
        Write-Host $Description -ForegroundColor Green
    }
    
    # Create a temporary script file
    $tempScript = [System.IO.Path]::GetTempFileName() + ".sh"
    $Command | Out-File -FilePath $tempScript -Encoding ASCII
    
    # Use SSH with password (requires sshpass or manual entry)
    $sshCmd = "ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null $SERVER_USER@$SERVER_IP 'bash -s' < $tempScript"
    
    Write-Host "Executing: $Command" -ForegroundColor Gray
    Write-Host "You may be prompted for password: $SERVER_PASS" -ForegroundColor Yellow
    
    # Execute
    bash -c $sshCmd
    
    Remove-Item $tempScript -ErrorAction SilentlyContinue
}

# Step 1: Test connection
Write-Host "`n=== Step 1: Testing Connection ===" -ForegroundColor Cyan
Write-Host "When prompted, enter password: $SERVER_PASS" -ForegroundColor Yellow
ssh -o StrictHostKeyChecking=no -o UserKnownHostsFile=/dev/null $SERVER_USER@$SERVER_IP "echo 'Connected successfully' && hostname && uname -a"

if ($LASTEXITCODE -ne 0) {
    Write-Host "Connection failed. Please ensure:" -ForegroundColor Red
    Write-Host "1. Server is accessible" -ForegroundColor Red
    Write-Host "2. SSH is enabled" -ForegroundColor Red
    Write-Host "3. Password is correct: $SERVER_PASS" -ForegroundColor Red
    Read-Host "Press Enter to try manual connection, or Ctrl+C to exit"
    
    Write-Host "`nTrying manual connection..." -ForegroundColor Yellow
    Write-Host "Password: $SERVER_PASS" -ForegroundColor Yellow
    ssh $SERVER_USER@$SERVER_IP
    exit
}

Write-Host "✓ Connection successful!" -ForegroundColor Green

# Step 2: Install Docker
Write-Host "`n=== Step 2: Installing Docker ===" -ForegroundColor Cyan
$dockerInstall = @"
echo '→ Updating system...'
apt-get update -qq

echo '→ Installing prerequisites...'
apt-get install -y -qq ca-certificates curl gnupg

echo '→ Adding Docker repository...'
install -m 0755 -d /etc/apt/keyrings
curl -fsSL https://download.docker.com/linux/ubuntu/gpg -o /etc/apt/keyrings/docker.asc
chmod a+r /etc/apt/keyrings/docker.asc

echo "deb [arch=\$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.asc] https://download.docker.com/linux/ubuntu \$(. /etc/os-release && echo \$VERSION_CODENAME) stable" | tee /etc/apt/sources.list.d/docker.list > /dev/null

echo '→ Installing Docker...'
apt-get update -qq
apt-get install -y -qq docker-ce docker-ce-cli containerd.io docker-buildx-plugin docker-compose-plugin

echo '→ Starting Docker...'
systemctl start docker
systemctl enable docker

echo '→ Docker installed:'
docker --version
docker compose version
"@

ssh $SERVER_USER@$SERVER_IP $dockerInstall

Write-Host "✓ Docker installed!" -ForegroundColor Green

# Step 3: Create directories
Write-Host "`n=== Step 3: Creating Directories ===" -ForegroundColor Cyan
ssh $SERVER_USER@$SERVER_IP "mkdir -p /opt/doganconsult && echo '✓ Directory created: /opt/doganconsult'"

# Step 4: Transfer files
Write-Host "`n=== Step 4: Transferring Files ===" -ForegroundColor Cyan
Write-Host "This may take several minutes depending on your connection..." -ForegroundColor Yellow

# Copy docker-compose file
Write-Host "→ Copying docker-compose.yml..." -ForegroundColor Gray
scp -o StrictHostKeyChecking=no docker-compose.prod.yml "${SERVER_USER}@${SERVER_IP}:/opt/doganconsult/docker-compose.yml"

# Copy source files
Write-Host "→ Copying source files..." -ForegroundColor Gray
scp -r -o StrictHostKeyChecking=no src "${SERVER_USER}@${SERVER_IP}:/opt/doganconsult/"

Write-Host "→ Copying configuration files..." -ForegroundColor Gray
scp -o StrictHostKeyChecking=no common.props "${SERVER_USER}@${SERVER_IP}:/opt/doganconsult/" 2>$null

Write-Host "✓ Files transferred!" -ForegroundColor Green

# Step 5: Build and start services
Write-Host "`n=== Step 5: Building and Starting Services ===" -ForegroundColor Cyan
Write-Host "This will take 10-15 minutes for first build..." -ForegroundColor Yellow

$buildCmd = @"
cd /opt/doganconsult
echo '→ Building Docker images...'
docker compose build
echo '→ Starting services...'
docker compose up -d
echo '→ Waiting for services to start...'
sleep 10
echo '→ Service status:'
docker compose ps
"@

ssh $SERVER_USER@$SERVER_IP $buildCmd

Write-Host "✓ Services started!" -ForegroundColor Green

# Step 6: Display results
Write-Host "`n=== Deployment Complete! ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "Your application is now running at:" -ForegroundColor White
Write-Host "  🌐 Blazor UI:        http://$SERVER_IP:5001" -ForegroundColor Yellow
Write-Host "  🔌 API Gateway:      http://$SERVER_IP:5000" -ForegroundColor Yellow
Write-Host "  🔐 Identity Service: http://$SERVER_IP:5002" -ForegroundColor Yellow
Write-Host ""
Write-Host "To view logs:" -ForegroundColor White
Write-Host "  ssh $SERVER_USER@$SERVER_IP 'cd /opt/doganconsult && docker compose logs -f'" -ForegroundColor Gray
Write-Host ""
Write-Host "To check status:" -ForegroundColor White
Write-Host "  ssh $SERVER_USER@$SERVER_IP 'cd /opt/doganconsult && docker compose ps'" -ForegroundColor Gray
Write-Host ""

Read-Host "Press Enter to exit"
