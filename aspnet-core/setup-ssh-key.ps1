# Setup SSH Key for Password-less Login
$IP = "46.224.64.95"
$USER = "root"
$PASS = "As`$123456"

Write-Host "=== SSH Key Setup ===" -ForegroundColor Cyan

# Check if key exists
$keyPath = "$env:USERPROFILE\.ssh\id_rsa"
if (-not (Test-Path $keyPath)) {
    Write-Host "Generating SSH key..." -ForegroundColor Green
    ssh-keygen -t rsa -b 4096 -f $keyPath -N '""'
}

# Copy key to server
Write-Host "Installing key on server..." -ForegroundColor Green
Write-Host "Password: $PASS" -ForegroundColor Yellow

$pubKey = Get-Content "$keyPath.pub"

# Use plink to add key
if (Test-Path "C:\Program Files\PuTTY\plink.exe") {
    $cmd = "mkdir -p ~/.ssh && echo '$pubKey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && chmod 700 ~/.ssh"
    & "C:\Program Files\PuTTY\plink.exe" -batch -pw $PASS "${USER}@${IP}" $cmd
} else {
    # Use OpenSSH
    $env:SSHPASS = $PASS
    $cmd = "mkdir -p ~/.ssh && echo '$pubKey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && chmod 700 ~/.ssh"
    echo $PASS | ssh -o StrictHostKeyChecking=no "${USER}@${IP}" $cmd
}

Write-Host ""
Write-Host "Done! Now you can connect without password:" -ForegroundColor Green
Write-Host "  ssh root@46.224.64.95" -ForegroundColor White
Write-Host ""
Write-Host "Or double-click: connect-server.bat" -ForegroundColor White
