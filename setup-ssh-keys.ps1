# Setup SSH keys and change password on Hetzner servers

$hetzner1IP = "46.224.64.95"
$hetzner1Password = "aKTbCKAeWapnp9xkLcjF"
$hetzner1NewPassword = "As`$1234565"

$hetzner2IP = "46.4.206.15"
$hetzner2Password = "PKdp_EM?cPFCh4"

$pubkey = Get-Content "$env:USERPROFILE\.ssh\id_rsa.pub"

Write-Host "=== Setting up SSH keys for Hetzner servers ===" -ForegroundColor Green

# Function to copy SSH key using password
function Copy-SSHKey {
    param(
        [string]$ServerIP,
        [string]$Password,
        [string]$PublicKey
    )
    
    Write-Host "`nSetting up SSH key for $ServerIP..." -ForegroundColor Yellow
    
    # Create a temporary script to run on remote server
    $script = @"
mkdir -p ~/.ssh
chmod 700 ~/.ssh
echo '$PublicKey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
echo "SSH key added successfully"
"@
    
    # Use plink or ssh with password (requires sshpass or expect)
    # For Windows, we'll use a different approach
    $script | Out-File -FilePath "$env:TEMP\setup_key.sh" -Encoding UTF8 -NoNewline
    
    Write-Host "  Copying key to $ServerIP..." -ForegroundColor Gray
    Write-Host "  Note: You may need to enter password manually" -ForegroundColor Gray
    
    # Try using ssh with password (may require manual input)
    ssh root@$ServerIP "mkdir -p ~/.ssh && chmod 700 ~/.ssh && echo '$PublicKey' >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys && echo 'Key added'"
}

# Setup hetzner1
Write-Host "`n[1/3] Setting up hetzner1 (46.224.64.95)..." -ForegroundColor Cyan
Write-Host "  Current password: $hetzner1Password" -ForegroundColor Gray
Write-Host "  New password will be: As`$1234565" -ForegroundColor Gray

# Create script to run on hetzner1
$hetzner1Script = @"
# Add SSH key
mkdir -p ~/.ssh
chmod 700 ~/.ssh
echo '$pubkey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys

# Change password
echo "root:As`$1234565" | chpasswd

echo "Setup complete on hetzner1"
"@

Write-Host "`nTo complete setup, run these commands manually:" -ForegroundColor Yellow
Write-Host "`nFor hetzner1 (46.224.64.95):" -ForegroundColor Cyan
Write-Host "ssh root@46.224.64.95" -ForegroundColor White
Write-Host "mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "echo 'root:As`$1234565' | chpasswd" -ForegroundColor White

Write-Host "`nFor hetzner2 (46.4.206.15):" -ForegroundColor Cyan
Write-Host "ssh root@46.4.206.15" -ForegroundColor White
Write-Host "mkdir -p ~/.ssh && chmod 700 ~/.ssh" -ForegroundColor White
Write-Host "echo '$pubkey' >> ~/.ssh/authorized_keys" -ForegroundColor White
Write-Host "chmod 600 ~/.ssh/authorized_keys" -ForegroundColor White

Write-Host "`nOr use this automated method:" -ForegroundColor Yellow
Write-Host "`$pubkey = Get-Content `$env:USERPROFILE\.ssh\id_rsa.pub" -ForegroundColor Gray
Write-Host "`$pubkey | ssh root@46.224.64.95 'mkdir -p ~/.ssh && chmod 700 ~/.ssh && cat >> ~/.ssh/authorized_keys && chmod 600 ~/.ssh/authorized_keys'" -ForegroundColor Gray
