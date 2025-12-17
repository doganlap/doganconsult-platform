# Automated SSH key copy and password change

param(
    [string]$ServerIP,
    [string]$Password,
    [string]$NewPassword = $null
)

$pubkey = Get-Content "$env:USERPROFILE\.ssh\id_rsa.pub"

Write-Host "Copying SSH key to $ServerIP..." -ForegroundColor Yellow

# Create expect-like script using here-string
$commands = @"
mkdir -p ~/.ssh
chmod 700 ~/.ssh
echo '$pubkey' >> ~/.ssh/authorized_keys
chmod 600 ~/.ssh/authorized_keys
"@

if ($NewPassword) {
    $commands += "`necho 'root:$NewPassword' | chpasswd`n"
    $commands += "echo 'Password changed successfully'`n"
}

$commands += "echo 'SSH key setup complete'`n"

# Save commands to temp file
$tempScript = "$env:TEMP\ssh_setup_$(Get-Random).sh"
$commands | Out-File -FilePath $tempScript -Encoding UTF8 -NoNewline

Write-Host "Commands prepared. Run manually:" -ForegroundColor Cyan
Write-Host "ssh root@$ServerIP" -ForegroundColor White
Write-Host "Then paste these commands:" -ForegroundColor Gray
Write-Host $commands -ForegroundColor Gray
