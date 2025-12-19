# Script to clean sensitive data from appsettings files
# Run this to replace production credentials with placeholders

$ErrorActionPreference = "Stop"

Write-Host "=== Securing Configuration Files ===" -ForegroundColor Green

$files = @(
    "src/DoganConsult.AI.HttpApi.Host/appsettings.json",
    "src/DoganConsult.AI.HttpApi.Host/appsettings.Development.json",
    "src/DoganConsult.Web.DbMigrator/appsettings.json",
    "src/DoganConsult.AI.DbMigrator/appsettings.json"
)

$replacements = @{
    # Database passwords
    "RRcrRrKgksUqapCckJJqBUIyWBhoNDJg" = "CHANGE_ME_AI_DB_PASSWORD"
    "mcmfdSTZUcDwqJtwkQXzJQSTurwVaQvz" = "CHANGE_ME_IDENTITY_DB_PASSWORD"
    "dEUIWJcxwajoHIoLwqTnAITbgXraTTKc" = "CHANGE_ME_ORG_DB_PASSWORD"
    "EwciEISIVdnEulryLkVgqHOVlaqjiyML" = "CHANGE_ME_WORKSPACE_DB_PASSWORD"
    "lKZKCQxdrBpklnirrzTSzznhzofmdNlv" = "CHANGE_ME_PROFILE_DB_PASSWORD"
    "PxccVRRaJCXlJGVdrzDTCGfApuJQEFVo" = "CHANGE_ME_AUDIT_DB_PASSWORD"
    "sCjZiToIMDSAznQNZZXcmAJlnLcElioE" = "CHANGE_ME_DOCUMENT_DB_PASSWORD"
    
    # Redis password
    "sOJrVPlSFlDQQpMizveGoYpFyzuNiPIv" = "CHANGE_ME_REDIS_PASSWORD"
    
    # Encryption passphrase
    "WLKovaoK9JsQQy1y" = "CHANGE_ME_ENCRYPTION_KEY"
}

foreach ($file in $files) {
    if (Test-Path $file) {
        Write-Host "  Securing: $file" -ForegroundColor Yellow
        $content = Get-Content $file -Raw
        
        foreach ($old in $replacements.Keys) {
            if ($content -match $old) {
                $new = $replacements[$old]
                $content = $content -replace [regex]::Escape($old), $new
                Write-Host "    ✓ Replaced sensitive value with $new" -ForegroundColor Green
            }
        }
        
        $content | Set-Content $file -NoNewline
    }
}

Write-Host "`n=== Configuration Secured ===" -ForegroundColor Green
Write-Host "NOTE: Update appsettings.Production.json with real values during deployment" -ForegroundColor Cyan
Write-Host "      Or use environment variables:" -ForegroundColor Cyan
Write-Host "      export ConnectionStrings__Default='...'`n" -ForegroundColor Gray

