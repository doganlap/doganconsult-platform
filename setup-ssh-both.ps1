# Complete SSH setup for both Hetzner servers
# This script generates commands and provides automated setup

$hetzner1IP = "46.224.64.95"
$hetzner1Password = "aKTbCKAeWapnp9xkLcjF"
$hetzner1NewPassword = "As`$1234565"

$hetzner2IP = "46.4.206.15"
$hetzner2Password = "PKdp_EM?cPFCh4"

$pubkey = Get-Content "$env:USERPROFILE\.ssh\id_rsa.pub"

Write-Host "`n=== SSH Setup for Both Hetzner Servers ===" -ForegroundColor Green
Write-Host ""

# Create batch file for easy execution
$batchContent = @"
@echo off
echo Setting up SSH keys for both Hetzner servers...
echo.

echo [1/2] Setting up hetzner1 (46.224.64.95)...
echo You will be prompted for password: aKTbCKAeWapnp9xkLcjF
echo.
ssh root@46.224.64.95 "mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'root:$hetzner1NewPassword' | chpasswd && echo 'Setup complete on hetzner1'"

echo.
echo [2/2] Setting up hetzner2 (46.4.206.15)...
echo You will be prompted for password: PKdp_EM?cPFCh4
echo.
ssh root@46.4.206.15 "mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'Setup complete on hetzner2'"

echo.
echo Setup complete! Test with: ssh hetzner1 or ssh hetzner2
pause
"@

$batchFile = "d:\test\setup-ssh.bat"
$batchContent | Out-File -FilePath $batchFile -Encoding ASCII

Write-Host "Created batch file: $batchFile" -ForegroundColor Green
Write-Host ""

# Also create individual scripts for each server
$hetzner1Script = @"
#!/bin/bash
# Setup script for hetzner1
mkdir -p ~/.ssh
chmod 700 ~/.ssh
echo '$pubkey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
echo 'root:$hetzner1NewPassword' | chpasswd
echo 'SSH key and password setup complete on hetzner1'
"@

$hetzner2Script = @"
#!/bin/bash
# Setup script for hetzner2
mkdir -p ~/.ssh
chmod 700 ~/.ssh
echo '$pubkey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
echo 'SSH key setup complete on hetzner2'
"@

$hetzner1Script | Out-File -FilePath "d:\test\setup-hetzner1.sh" -Encoding UTF8 -NoNewline
$hetzner2Script | Out-File -FilePath "d:\test\setup-hetzner2.sh" -Encoding UTF8 -NoNewline

Write-Host "Created setup scripts:" -ForegroundColor Green
Write-Host "  - setup-ssh.bat (Windows batch file)" -ForegroundColor Cyan
Write-Host "  - setup-hetzner1.sh (for hetzner1)" -ForegroundColor Cyan
Write-Host "  - setup-hetzner2.sh (for hetzner2)" -ForegroundColor Cyan
Write-Host ""

Write-Host "=== Quick Setup Instructions ===" -ForegroundColor Yellow
Write-Host ""
Write-Host "Option 1: Run the batch file (easiest)" -ForegroundColor Cyan
Write-Host "  .\setup-ssh.bat" -ForegroundColor White
Write-Host ""

Write-Host "Option 2: Run commands manually" -ForegroundColor Cyan
Write-Host ""
Write-Host "For hetzner1 ($hetzner1IP):" -ForegroundColor Yellow
Write-Host "  ssh root@$hetzner1IP" -ForegroundColor White
Write-Host "  (Password: $hetzner1Password)" -ForegroundColor Gray
Write-Host "  Then run:" -ForegroundColor Gray
Write-Host "  mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "  echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "  chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "  echo 'root:$hetzner1NewPassword' | chpasswd" -ForegroundColor White
Write-Host ""

Write-Host "For hetzner2 ($hetzner2IP):" -ForegroundColor Yellow
Write-Host "  ssh root@$hetzner2IP" -ForegroundColor White
Write-Host "  (Password: $hetzner2Password)" -ForegroundColor Gray
Write-Host "  Then run:" -ForegroundColor Gray
Write-Host "  mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "  echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "  chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host ""

Write-Host "Option 3: Copy scripts to servers" -ForegroundColor Cyan
Write-Host "  scp setup-hetzner1.sh root@$hetzner1IP:/tmp/" -ForegroundColor White
Write-Host "  ssh root@$hetzner1IP 'bash /tmp/setup-hetzner1.sh'" -ForegroundColor White
Write-Host "  scp setup-hetzner2.sh root@$hetzner2IP:/tmp/" -ForegroundColor White
Write-Host "  ssh root@$hetzner2IP 'bash /tmp/setup-hetzner2.sh'" -ForegroundColor White
Write-Host ""

Write-Host "After setup, test with:" -ForegroundColor Green
Write-Host "  ssh hetzner1" -ForegroundColor Cyan
Write-Host "  ssh hetzner2" -ForegroundColor Cyan
