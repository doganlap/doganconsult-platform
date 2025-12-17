# Simple Deployment Script
$SERVER_IP = "46.224.64.95"
$SERVER_USER = "root"
$SERVER_PASS = "aKTbCKAeWapnp9xkLcjF"

Write-Host "=== DoganConsult Auto Deploy ===" -ForegroundColor Cyan

# Download WinSCP Portable
Write-Host "Downloading WinSCP..." -ForegroundColor Green
$winscp = "$env:TEMP\winscp.zip"
Invoke-WebRequest -Uri "https://winscp.net/download/WinSCP-6.3.5-Portable.zip" -OutFile $winscp
Expand-Archive -Path $winscp -DestinationPath "$env:TEMP\WinSCP" -Force

# Create transfer script
$scriptContent = "open sftp://${SERVER_USER}:${SERVER_PASS}@${SERVER_IP}/`n"
$scriptContent += "cd /opt/doganconsult`n"
$scriptContent += "put docker-compose.prod.yml docker-compose.yml`n"
$scriptContent += "put common.props`n"
$scriptContent += "lcd src`n"
$scriptContent += "cd /opt/doganconsult`n"
$scriptContent += "mkdir src`n"
$scriptContent += "cd src`n"
$scriptContent += "put -r *`n"
$scriptContent += "close`n"
$scriptContent += "exit`n"

$scriptContent | Out-File "$env:TEMP\winscp-script.txt" -Encoding ASCII

# Transfer files
Write-Host "Transferring files..." -ForegroundColor Green
& "$env:TEMP\WinSCP\WinSCP.com" /script="$env:TEMP\winscp-script.txt"

# Create server setup script
$serverScript = "$env:TEMP\setup-server.sh"
@'
#!/bin/bash
cd /opt/doganconsult
docker compose build
docker compose up -d
docker compose ps
'@ | Out-File $serverScript -Encoding UTF8

# Execute on server
Write-Host "Starting services..." -ForegroundColor Green
if (Test-Path "C:\Program Files\PuTTY\pscp.exe") {
    & "C:\Program Files\PuTTY\pscp.exe" -batch -pw $SERVER_PASS $serverScript root@${SERVER_IP}:/root/deploy.sh
    & "C:\Program Files\PuTTY\plink.exe" -batch -pw $SERVER_PASS root@$SERVER_IP "chmod +x /root/deploy.sh && /root/deploy.sh"
}

# Create shortcuts
$desktop = [Environment]::GetFolderPath("Desktop")

# RDP
"full address:s:$SERVER_IP`:3389`nusername:s:$SERVER_USER" | Out-File "$desktop\DoganConsult-Server.rdp" -Encoding ASCII

# URLs
"[InternetShortcut]`nURL=http://$SERVER_IP`:5001" | Out-File "$desktop\DoganConsult-Platform.url" -Encoding ASCII
"[InternetShortcut]`nURL=http://$SERVER_IP`:10000" | Out-File "$desktop\DoganConsult-Webmin.url" -Encoding ASCII

Write-Host ""
Write-Host "Done! Platform: http://$SERVER_IP`:5001" -ForegroundColor Green
Start-Process "http://$SERVER_IP`:5001"
