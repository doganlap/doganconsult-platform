# Complete Automated Setup for DoganConsult Platform
# Downloads tools, configures server, deploys platform

$ErrorActionPreference = "Stop"

$SERVER_IP = "46.224.64.95"
$SERVER_USER = "root"
$SERVER_PASS = "aKTbCKAeWapnp9xkLcjF"

Write-Host "=== DoganConsult Platform - Complete Auto Setup ===" -ForegroundColor Cyan
Write-Host ""

# Create temp directory
$tempDir = "$env:TEMP\DoganConsult-Setup"
New-Item -ItemType Directory -Force -Path $tempDir | Out-Null

# Step 1: Download WinSCP
Write-Host "Step 1: Downloading WinSCP..." -ForegroundColor Green
$winscpUrl = "https://winscp.net/download/WinSCP-6.3.5-Portable.zip"
$winscpZip = "$tempDir\winscp.zip"
$winscpDir = "$tempDir\WinSCP"

if (-not (Test-Path "C:\Program Files (x86)\WinSCP\WinSCP.exe")) {
    Invoke-WebRequest -Uri $winscpUrl -OutFile $winscpZip
    Expand-Archive -Path $winscpZip -DestinationPath $winscpDir -Force
    Write-Host "  ✓ WinSCP downloaded" -ForegroundColor Gray
} else {
    $winscpDir = "C:\Program Files (x86)\WinSCP"
    Write-Host "  ✓ WinSCP already installed" -ForegroundColor Gray
}

# Step 2: Download MobaXterm
Write-Host "Step 2: Downloading MobaXterm..." -ForegroundColor Green
$mobaUrl = "https://download.mobatek.net/2432024120924733/MobaXterm_Portable_v24.3.zip"
$mobaZip = "$tempDir\mobaxterm.zip"
$mobaDir = "$tempDir\MobaXterm"

Invoke-WebRequest -Uri $mobaUrl -OutFile $mobaZip -ErrorAction SilentlyContinue
if (Test-Path $mobaZip) {
    Expand-Archive -Path $mobaZip -DestinationPath $mobaDir -Force
    Write-Host "  ✓ MobaXterm downloaded" -ForegroundColor Gray
}

# Step 3: Create WinSCP script for file transfer
Write-Host "Step 3: Preparing file transfer..." -ForegroundColor Green
$winscpScript = @"
option batch abort
option confirm off
open sftp://${SERVER_USER}:${SERVER_PASS}@${SERVER_IP}/
cd /opt/doganconsult
put "$($PWD.Path)\docker-compose.prod.yml" docker-compose.yml
put "$($PWD.Path)\common.props"
lcd src
put -r * /opt/doganconsult/src/
close
exit
"@

$scriptPath = "$tempDir\transfer.txt"
$winscpScript | Out-File -FilePath $scriptPath -Encoding ASCII

# Step 4: Transfer files using WinSCP
Write-Host "Step 4: Transferring files to server..." -ForegroundColor Green
$winscpExe = Get-ChildItem -Path $winscpDir -Filter "WinSCP.com" -Recurse | Select-Object -First 1

if ($winscpExe) {
    & $winscpExe.FullName /script="$scriptPath" /log="$tempDir\transfer.log"
    Write-Host "  ✓ Files transferred" -ForegroundColor Gray
} else {
    Write-Host "  ⚠ WinSCP not found, using alternative method" -ForegroundColor Yellow
}

# Step 5: Configure server with GUI tools
Write-Host "Step 5: Configuring server with GUI tools..." -ForegroundColor Green

$serverSetupScript = @'
#!/bin/bash
set -e

echo "Installing GUI and management tools..."

# Update system
apt-get update

# Install XFCE Desktop
DEBIAN_FRONTEND=noninteractive apt-get install -y xfce4 xfce4-goodies xorg dbus-x11

# Install XRDP (Remote Desktop)
apt-get install -y xrdp
echo "xfce4-session" > ~/.xsession
systemctl enable xrdp
systemctl start xrdp

# Install Webmin
curl -fsSL https://download.webmin.com/jcameron-key.asc | gpg --dearmor -o /usr/share/keyrings/webmin.gpg
echo "deb [signed-by=/usr/share/keyrings/webmin.gpg] https://download.webmin.com/download/repository sarge contrib" > /etc/apt/sources.list.d/webmin.list
apt-get update
apt-get install -y webmin

# Install Cockpit
apt-get install -y cockpit cockpit-docker
systemctl enable --now cockpit.socket

# Install Code Server (VS Code in browser)
curl -fsSL https://code-server.dev/install.sh | sh
mkdir -p ~/.config/code-server
cat > ~/.config/code-server/config.yaml << 'EOFCODE'
bind-addr: 0.0.0.0:8080
auth: password
password: admin123
cert: false
EOFCODE
systemctl enable --now code-server@root

# Install Samba for network drive
apt-get install -y samba
cat > /etc/samba/smb.conf << 'EOFSMB'
[global]
workgroup = WORKGROUP
server string = DoganConsult Server
security = user

[DoganConsult]
path = /opt/doganconsult
browseable = yes
writable = yes
valid users = root
create mask = 0755
EOFSMB

echo -e "samba123\nsamba123" | smbpasswd -a root
systemctl restart smbd

# Install Portainer (Docker UI)
docker run -d \
  --name portainer \
  --restart=always \
  -p 9000:9000 \
  -p 9443:9443 \
  -v /var/run/docker.sock:/var/run/docker.sock \
  -v portainer_data:/data \
  portainer/portainer-ce:latest

# Configure firewall
ufw allow 22/tcp    # SSH
ufw allow 3389/tcp  # RDP
ufw allow 10000/tcp # Webmin
ufw allow 9090/tcp  # Cockpit
ufw allow 8080/tcp  # Code Server
ufw allow 9000/tcp  # Portainer
ufw allow 9443/tcp  # Portainer HTTPS
ufw allow 445/tcp   # Samba
ufw allow 139/tcp   # Samba
ufw allow 5000:5008/tcp  # DoganConsult services
ufw --force enable

echo "GUI setup complete!"
'@

$setupScriptPath = "$tempDir\server-setup.sh"
$serverSetupScript | Out-File -FilePath $setupScriptPath -Encoding UTF8

# Transfer and execute setup script
Write-Host "  → Installing GUI tools on server..." -ForegroundColor Gray

if (Test-Path "C:\Program Files\PuTTY\plink.exe") {
    $plinkPath = "C:\Program Files\PuTTY\plink.exe"
    
    # Transfer setup script
    & "C:\Program Files\PuTTY\pscp.exe" -batch -pw $SERVER_PASS $setupScriptPath "${SERVER_USER}@${SERVER_IP}:/root/server-setup.sh"
    
    # Execute setup script
    $commands = @(
        "chmod +x /root/server-setup.sh",
        "bash /root/server-setup.sh"
    )
    
    foreach ($cmd in $commands) {
        & $plinkPath -batch -pw $SERVER_PASS "${SERVER_USER}@${SERVER_IP}" $cmd
    }
    
    Write-Host "  ✓ Server configured" -ForegroundColor Gray
}

# Step 6: Deploy platform
Write-Host "Step 6: Deploying DoganConsult platform..." -ForegroundColor Green

$deployCommands = @"
cd /opt/doganconsult
docker compose build --parallel
docker compose up -d
docker compose ps
"@

if (Test-Path $plinkPath) {
    & $plinkPath -batch -pw $SERVER_PASS "${SERVER_USER}@${SERVER_IP}" $deployCommands
    Write-Host "  ✓ Platform deployed" -ForegroundColor Gray
}

# Step 7: Create desktop shortcuts
Write-Host "Step 7: Creating shortcuts..." -ForegroundColor Green

$desktopPath = [Environment]::GetFolderPath("Desktop")

# RDP Shortcut
$rdpContent = @"
full address:s:${SERVER_IP}:3389
username:s:${SERVER_USER}
"@
$rdpContent | Out-File -FilePath "$desktopPath\DoganConsult Server.rdp" -Encoding ASCII

# Web shortcuts
$shortcuts = @{
    "DoganConsult Webmin" = "http://${SERVER_IP}:10000"
    "DoganConsult Cockpit" = "http://${SERVER_IP}:9090"
    "DoganConsult VS Code" = "http://${SERVER_IP}:8080"
    "DoganConsult Portainer" = "http://${SERVER_IP}:9000"
    "DoganConsult Platform" = "http://${SERVER_IP}:5001"
}

foreach ($name in $shortcuts.Keys) {
    $url = $shortcuts[$name]
    $shortcutPath = "$desktopPath\$name.url"
    "[InternetShortcut]`nURL=$url" | Out-File -FilePath $shortcutPath -Encoding ASCII
}

Write-Host "  ✓ Shortcuts created on desktop" -ForegroundColor Gray

# Summary
Write-Host ""
Write-Host "=== Setup Complete! ===" -ForegroundColor Cyan
Write-Host ""
Write-Host "📁 Desktop Shortcuts Created:" -ForegroundColor White
Write-Host "  • DoganConsult Server.rdp - Remote Desktop" -ForegroundColor Yellow
Write-Host "  • DoganConsult Webmin.url - Server Management" -ForegroundColor Yellow
Write-Host "  • DoganConsult Cockpit.url - System Dashboard" -ForegroundColor Yellow
Write-Host "  • DoganConsult VS Code.url - Code Editor" -ForegroundColor Yellow
Write-Host "  • DoganConsult Portainer.url - Docker Manager" -ForegroundColor Yellow
Write-Host "  • DoganConsult Platform.url - Your Application" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔐 Credentials:" -ForegroundColor White
Write-Host "  RDP: root / $SERVER_PASS" -ForegroundColor Gray
Write-Host "  Webmin: root / $SERVER_PASS" -ForegroundColor Gray
Write-Host "  VS Code: admin123" -ForegroundColor Gray
Write-Host "  Samba: root / samba123" -ForegroundColor Gray
Write-Host ""
Write-Host "📂 Network Drive:" -ForegroundColor White
Write-Host "  \\${SERVER_IP}\DoganConsult" -ForegroundColor Gray
Write-Host ""
Write-Host "🚀 DoganConsult Platform:" -ForegroundColor White
Write-Host "  UI: http://${SERVER_IP}:5001" -ForegroundColor Green
Write-Host "  API: http://${SERVER_IP}:5000" -ForegroundColor Green
Write-Host ""

# Open platform in browser
Start-Process "http://${SERVER_IP}:5001"

Read-Host "Press Enter to finish"
