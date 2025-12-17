# Automated setup for both Hetzner servers
# Sets up SSH keys and changes password on hetzner1

$ErrorActionPreference = "Stop"

$hetzner1IP = "46.224.64.95"
$hetzner1Password = "aKTbCKAeWapnp9xkLcjF"
$hetzner1NewPassword = "As`$1234565"

$hetzner2IP = "46.4.206.15"
$hetzner2Password = "PKdp_EM?cPFCh4"

$pubkey = Get-Content "$env:USERPROFILE\.ssh\id_rsa.pub"

Write-Host "=== Setting up both Hetzner servers ===" -ForegroundColor Green
Write-Host "hetzner1: $hetzner1IP" -ForegroundColor Cyan
Write-Host "hetzner2: $hetzner2IP" -ForegroundColor Cyan
Write-Host ""

# Function to setup SSH key on a server
function Setup-SSHKey {
    param(
        [string]$ServerIP,
        [string]$Password,
        [string]$PublicKey,
        [string]$NewPassword = $null
    )
    
    Write-Host "Setting up $ServerIP..." -ForegroundColor Yellow
    
    # Create setup script
    $setupScript = @"
#!/bin/bash
set -e

# Create .ssh directory
mkdir -p ~/.ssh
chmod 700 ~/.ssh

# Add public key
echo '$PublicKey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys

# Change password if provided
"@
    
    if ($NewPassword) {
        $setupScript += "echo 'root:$NewPassword' | chpasswd`n"
        $setupScript += "echo 'Password changed to: $NewPassword'`n"
    }
    
    $setupScript += "echo 'SSH key setup complete on $ServerIP'`n"
    
    # Save script to temp file
    $tempFile = "$env:TEMP\setup_$($ServerIP.Replace('.','_')).sh"
    $setupScript | Out-File -FilePath $tempFile -Encoding UTF8 -NoNewline
    
    Write-Host "  Script created: $tempFile" -ForegroundColor Gray
    
    return $tempFile
}

# Create setup scripts
Write-Host "`n[1/4] Creating setup scripts..." -ForegroundColor Yellow
$hetzner1Script = Setup-SSHKey -ServerIP $hetzner1IP -Password $hetzner1Password -PublicKey $pubkey -NewPassword $hetzner1NewPassword
$hetzner2Script = Setup-SSHKey -ServerIP $hetzner2IP -Password $hetzner2Password -PublicKey $pubkey

# Copy scripts to servers and execute
Write-Host "`n[2/4] Copying scripts to servers..." -ForegroundColor Yellow

# For hetzner1 - we'll use a different approach since we need to provide password
Write-Host "`n[3/4] Setting up hetzner1 ($hetzner1IP)..." -ForegroundColor Cyan
Write-Host "  Note: You'll need to enter password: $hetzner1Password" -ForegroundColor Gray

$hetzner1Commands = @"
mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'root:$hetzner1NewPassword' | chpasswd && echo 'Setup complete on hetzner1'
"@

Write-Host "  Running commands on hetzner1..." -ForegroundColor Gray
try {
    $hetzner1Commands | ssh root@$hetzner1IP bash 2>&1
    Write-Host "  ✓ hetzner1 setup complete" -ForegroundColor Green
} catch {
    Write-Host "  ⚠ hetzner1 setup may require manual password entry" -ForegroundColor Yellow
    Write-Host "  Run manually: ssh root@$hetzner1IP" -ForegroundColor Gray
    Write-Host "  Then: $hetzner1Commands" -ForegroundColor Gray
}

Write-Host "`n[4/4] Setting up hetzner2 ($hetzner2IP)..." -ForegroundColor Cyan
Write-Host "  Note: You'll need to enter password: $hetzner2Password" -ForegroundColor Gray

$hetzner2Commands = @"
mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$pubkey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'Setup complete on hetzner2'
"@

Write-Host "  Running commands on hetzner2..." -ForegroundColor Gray
try {
    $hetzner2Commands | ssh root@$hetzner2IP bash 2>&1
    Write-Host "  ✓ hetzner2 setup complete" -ForegroundColor Green
} catch {
    Write-Host "  ⚠ hetzner2 setup may require manual password entry" -ForegroundColor Yellow
    Write-Host "  Run manually: ssh root@$hetzner2IP" -ForegroundColor Gray
    Write-Host "  Then: $hetzner2Commands" -ForegroundColor Gray
}

Write-Host "`n=== Setup Complete ===" -ForegroundColor Green
Write-Host "`nTest connections:" -ForegroundColor Yellow
Write-Host "  ssh hetzner1" -ForegroundColor Cyan
Write-Host "  ssh hetzner2" -ForegroundColor Cyan

Write-Host "`nIf passwordless SSH doesn't work, run these commands manually:" -ForegroundColor Yellow

Write-Host "`nFor hetzner1:" -ForegroundColor Cyan
Write-Host "ssh root@$hetzner1IP" -ForegroundColor White
Write-Host "mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "echo 'root:$hetzner1NewPassword' | chpasswd" -ForegroundColor White

Write-Host "`nFor hetzner2:" -ForegroundColor Cyan
Write-Host "ssh root@$hetzner2IP" -ForegroundColor White
Write-Host "mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White
