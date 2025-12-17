# Create PuTTY Saved Session for One-Click Access
$IP = "46.224.64.95"
$USER = "root"
$SESSION_NAME = "DoganConsult-Server"

Write-Host "Creating PuTTY session: $SESSION_NAME" -ForegroundColor Cyan

# PuTTY registry path
$regPath = "HKCU:\Software\SimonTatham\PuTTY\Sessions\$SESSION_NAME"

# Create registry keys
New-Item -Path $regPath -Force | Out-Null

# Configure session
Set-ItemProperty -Path $regPath -Name "HostName" -Value $IP
Set-ItemProperty -Path $regPath -Name "UserName" -Value $USER
Set-ItemProperty -Path $regPath -Name "Protocol" -Value "ssh"
Set-ItemProperty -Path $regPath -Name "PortNumber" -Value 22
Set-ItemProperty -Path $regPath -Name "TerminalType" -Value "xterm"
Set-ItemProperty -Path $regPath -Name "CloseOnExit" -Value 0

Write-Host "PuTTY session created!" -ForegroundColor Green
Write-Host ""
Write-Host "To connect:" -ForegroundColor White
Write-Host "  1. Open PuTTY" -ForegroundColor Yellow
Write-Host "  2. Select '$SESSION_NAME' from Saved Sessions" -ForegroundColor Yellow
Write-Host "  3. Click 'Load' then 'Open'" -ForegroundColor Yellow
Write-Host ""
Write-Host "Or run: putty -load $SESSION_NAME" -ForegroundColor Green

# Create desktop shortcut
$WshShell = New-Object -ComObject WScript.Shell
$desktop = [Environment]::GetFolderPath("Desktop")
$shortcut = $WshShell.CreateShortcut("$desktop\DoganConsult Server.lnk")
$shortcut.TargetPath = "C:\Program Files\PuTTY\putty.exe"
$shortcut.Arguments = "-load `"$SESSION_NAME`""
$shortcut.IconLocation = "C:\Program Files\PuTTY\putty.exe,0"
$shortcut.Save()

Write-Host "Desktop shortcut created: DoganConsult Server.lnk" -ForegroundColor Green
