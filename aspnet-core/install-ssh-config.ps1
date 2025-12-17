# Install SSH Config for Easy Server Access
$sshDir = "$env:USERPROFILE\.ssh"
$configFile = "$sshDir\config"

# Create .ssh directory if it doesn't exist
if (-not (Test-Path $sshDir)) {
    New-Item -ItemType Directory -Path $sshDir -Force | Out-Null
}

# Copy config
Copy-Item ".\ssh-config" $configFile -Force

Write-Host "SSH config installed!" -ForegroundColor Green
Write-Host ""
Write-Host "Now you can connect using aliases:" -ForegroundColor Cyan
Write-Host "  ssh doganconsult" -ForegroundColor Yellow
Write-Host "  ssh hetzner1" -ForegroundColor Yellow
Write-Host "  ssh hetzner2" -ForegroundColor Yellow
Write-Host "  ssh azure" -ForegroundColor Yellow
Write-Host "  ssh ai-server" -ForegroundColor Yellow
Write-Host ""
Write-Host "Config file: $configFile" -ForegroundColor Gray
