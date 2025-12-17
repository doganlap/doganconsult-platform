# Create Desktop Shortcuts for DoganConsult Server
$desktop = [Environment]::GetFolderPath("Desktop")
$IP = "46.224.64.95"

Write-Host "Creating desktop shortcuts..." -ForegroundColor Cyan

# 1. SSH Connection Batch File
$sshBat = @"
@echo off
title DoganConsult Server SSH
color 0A
echo ================================================
echo   DoganConsult Server - SSH Connection
echo ================================================
echo.
echo   Server: $IP
echo   User: root
echo   Password: As`$123456
echo.
echo ================================================
echo.
ssh root@$IP
pause
"@

$sshBat | Out-File "$desktop\SSH-DoganConsult-Server.bat" -Encoding ASCII

# 2. Platform URL Shortcut
$platformUrl = @"
[InternetShortcut]
URL=http://$IP:5001
"@
$platformUrl | Out-File "$desktop\DoganConsult-Platform.url" -Encoding ASCII

# 3. RDP Connection
$rdp = @"
full address:s:${IP}:3389
username:s:root
"@
$rdp | Out-File "$desktop\DoganConsult-RDP.rdp" -Encoding ASCII

# 4. Webmin URL
$webmin = @"
[InternetShortcut]
URL=http://$IP:10000
"@
$webmin | Out-File "$desktop\DoganConsult-Webmin.url" -Encoding ASCII

# 5. Portainer URL
$portainer = @"
[InternetShortcut]
URL=http://$IP:9000
"@
$portainer | Out-File "$desktop\DoganConsult-Portainer.url" -Encoding ASCII

Write-Host ""
Write-Host "Desktop shortcuts created:" -ForegroundColor Green
Write-Host "  SSH-DoganConsult-Server.bat" -ForegroundColor White
Write-Host "  DoganConsult-Platform.url (Port 5001)" -ForegroundColor White
Write-Host "  DoganConsult-RDP.rdp (Remote Desktop)" -ForegroundColor White
Write-Host "  DoganConsult-Webmin.url (Port 10000)" -ForegroundColor White
Write-Host "  DoganConsult-Portainer.url (Port 9000)" -ForegroundColor White
Write-Host ""
Write-Host "Double-click SSH-DoganConsult-Server.bat to connect!" -ForegroundColor Yellow
