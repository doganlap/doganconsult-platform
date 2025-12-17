# Automated setup for both Hetzner servers
# Sets up SSH keys and changes password on hetzner1

$ErrorActionPreference = "Stop"

$hetzner1IP = "46.224.64.95"
$hetzner1Password = "aKTbCKAeWapnp9xkLcjF"
$hetzner1NewPassword = "As`$123456"

$hetzner2IP = "46.4.206.15"
$hetzner2Password = "PKdp_EM?cPFCh4"

# Check if SSH key exists
if (-not (Test-Path "$env:USERPROFILE\.ssh\id_rsa.pub")) {
    Write-Host "Generating SSH key..." -ForegroundColor Yellow
    ssh-keygen -t rsa -b 4096 -f "$env:USERPROFILE\.ssh\id_rsa" -N '""'
}

$pubkey = Get-Content "$env:USERPROFILE\.ssh\id_rsa.pub"

Write-Host "=== Setting up both Hetzner servers ===" -ForegroundColor Green
Write-Host "hetzner1 (doganconsult): $hetzner1IP" -ForegroundColor Cyan
Write-Host "hetzner2: $hetzner2IP" -ForegroundColor Cyan
Write-Host ""

# Setup hetzner1
Write-Host "[1/2] Setting up hetzner1 ($hetzner1IP)..." -ForegroundColor Cyan
Write-Host "  Password: $hetzner1Password" -ForegroundColor Gray

$hetzner1Commands = "mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'root:$hetzner1NewPassword' | chpasswd && echo 'Setup complete on hetzner1'"

Write-Host "  Installing SSH key and changing password..." -ForegroundColor Gray
try {
    echo $hetzner1Password | ssh -o StrictHostKeyChecking=no root@$hetzner1IP $hetzner1Commands 2>&1
    Write-Host "  ✓ hetzner1 setup complete" -ForegroundColor Green
    Write-Host "  ✓ Password changed to: $hetzner1NewPassword" -ForegroundColor Green
} catch {
    Write-Host "  ⚠ Automated setup failed. Manual setup required." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Run manually:" -ForegroundColor White
    Write-Host "  ssh root@$hetzner1IP" -ForegroundColor Cyan
    Write-Host "  Password: $hetzner1Password" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Then run these commands:" -ForegroundColor White
    Write-Host "  mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor Cyan
    Write-Host "  echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor Cyan
    Write-Host "  chmod 600 ~/.ssh/authorized_keys" -ForegroundColor Cyan
    Write-Host "  echo 'root:$hetzner1NewPassword' | chpasswd" -ForegroundColor Cyan
}

# Setup hetzner2
Write-Host ""
Write-Host "[2/2] Setting up hetzner2 ($hetzner2IP)..." -ForegroundColor Cyan
Write-Host "  Password: $hetzner2Password" -ForegroundColor Gray

$hetzner2Commands = "mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'Setup complete on hetzner2'"

Write-Host "  Installing SSH key..." -ForegroundColor Gray
try {
    echo $hetzner2Password | ssh -o StrictHostKeyChecking=no root@$hetzner2IP $hetzner2Commands 2>&1
    Write-Host "  ✓ hetzner2 setup complete" -ForegroundColor Green
} catch {
    Write-Host "  ⚠ Automated setup failed. Manual setup required." -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  Run manually:" -ForegroundColor White
    Write-Host "  ssh root@$hetzner2IP" -ForegroundColor Cyan
    Write-Host "  Password: $hetzner2Password" -ForegroundColor Gray
    Write-Host ""
    Write-Host "  Then run these commands:" -ForegroundColor White
    Write-Host "  mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor Cyan
    Write-Host "  echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor Cyan
    Write-Host "  chmod 600 ~/.ssh/authorized_keys" -ForegroundColor Cyan
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Test passwordless connections:" -ForegroundColor Yellow
Write-Host "  ssh doganconsult" -ForegroundColor Cyan
Write-Host "  ssh hetzner2" -ForegroundColor Cyan
Write-Host ""
Write-Host "Server credentials:" -ForegroundColor White
Write-Host "  hetzner1: root@$hetzner1IP (password: $hetzner1NewPassword)" -ForegroundColor Gray
Write-Host "  hetzner2: root@$hetzner2IP (password: $hetzner2Password)" -ForegroundColor Gray
Write-Host ""

Read-Host "Press Enter to close"
