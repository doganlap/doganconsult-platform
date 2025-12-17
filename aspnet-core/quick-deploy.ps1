# Quick Deploy to Hetzner
$IP = "46.224.64.95"
$U = "root"
$P = "As$123456"

Write-Host "DoganConsult Deploy Starting..." -ForegroundColor Cyan

# Download WinSCP
$zip = "$env:TEMP\winscp.zip"
Write-Host "Getting WinSCP..." -ForegroundColor Green
Invoke-WebRequest "https://winscp.net/download/WinSCP-6.3.5-Portable.zip" -OutFile $zip
Expand-Archive $zip "$env:TEMP\WinSCP" -Force

# Build WinSCP script
$cmd = "open sftp://" + $U + ":" + $P + "@" + $IP + "/" + [Environment]::NewLine
$cmd += "cd /opt/doganconsult" + [Environment]::NewLine
$cmd += "put docker-compose.prod.yml docker-compose.yml" + [Environment]::NewLine
$cmd += "put common.props" + [Environment]::NewLine
$cmd += "lcd src" + [Environment]::NewLine
$cmd += "put -r * /opt/doganconsult/src/" + [Environment]::NewLine
$cmd += "exit" + [Environment]::NewLine

$cmd | Out-File "$env:TEMP\transfer.txt" -Encoding ASCII

# Transfer
Write-Host "Uploading files..." -ForegroundColor Green
& "$env:TEMP\WinSCP\WinSCP.com" /script="$env:TEMP\transfer.txt"

# Deploy
Write-Host "Starting Docker..." -ForegroundColor Green
if (Test-Path "C:\Program Files\PuTTY\plink.exe") {
    & "C:\Program Files\PuTTY\plink.exe" -batch -pw $P "${U}@${IP}" "cd /opt/doganconsult && docker compose build && docker compose up -d"
}

# Shortcuts
$desk = [Environment]::GetFolderPath("Desktop")
"[InternetShortcut]`nURL=http://${IP}:5001" | Out-File "$desk\DoganConsult.url" -Encoding ASCII

Write-Host "Done! Opening http://${IP}:5001" -ForegroundColor Green
Start-Process "http://${IP}:5001"
