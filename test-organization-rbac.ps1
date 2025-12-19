# Organization Service RBAC Testing Script
# Tests authorization for Organization Service API endpoints

Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Organization Service RBAC Testing" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

# Configuration
$identityUrl = "https://localhost:44346"
$orgServiceUrl = "https://localhost:44337"
$adminUsername = "admin"
$adminPassword = "1q2w3E***"
$clientId = "DoganConsult_Blazor"
$clientSecret = "MySecret"

# Test Results
$testResults = @()

function Add-TestResult {
    param(
        [string]$TestName,
        [bool]$Passed,
        [string]$Details,
        [int]$StatusCode = 0
    )
    
    $testResults += [PSCustomObject]@{
        Test = $TestName
        Result = if ($Passed) { "✅ PASS" } else { "❌ FAIL" }
        StatusCode = $StatusCode
        Details = $Details
    }
    
    if ($Passed) {
        Write-Host "✅ $TestName" -ForegroundColor Green
        Write-Host "   $Details" -ForegroundColor White
    } else {
        Write-Host "❌ $TestName" -ForegroundColor Red
        Write-Host "   $Details" -ForegroundColor Yellow
    }
}

# Test 1: Get Admin Access Token
Write-Host "`n[Test 1] Getting Admin Access Token..." -ForegroundColor Yellow

try {
    $tokenResponse = Invoke-RestMethod -Uri "$identityUrl/connect/token" `
        -Method POST `
        -ContentType "application/x-www-form-urlencoded" `
        -Body @{
            grant_type = "password"
            username = $adminUsername
            password = $adminPassword
            client_id = $clientId
            client_secret = $clientSecret
            scope = "OrganizationService offline_access"
        } `
        -SkipCertificateCheck -ErrorAction Stop

    $adminToken = $tokenResponse.access_token
    Add-TestResult -TestName "Admin Authentication" -Passed $true -Details "Token received: $($adminToken.Substring(0, 30))..." -StatusCode 200
} catch {
    Add-TestResult -TestName "Admin Authentication" -Passed $false -Details "Failed to get token: $($_.Exception.Message)" -StatusCode 401
    Write-Host "`n⚠️  Cannot proceed without admin token. Exiting." -ForegroundColor Red
    exit 1
}

# Test 2: Admin GET Organizations (Should work - admin has all permissions)
Write-Host "`n[Test 2] Admin GET Organizations..." -ForegroundColor Yellow

try {
    $headers = @{
        Authorization = "Bearer $adminToken"
        Accept = "application/json"
    }
    
    $orgsResult = Invoke-RestMethod -Uri "$orgServiceUrl/api/organization-service/organizations" `
        -Method GET `
        -Headers $headers `
        -SkipCertificateCheck -ErrorAction Stop
    
    Add-TestResult -TestName "Admin GET Organizations" -Passed $true -Details "Total: $($orgsResult.totalCount) organizations" -StatusCode 200
} catch {
    $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
    Add-TestResult -TestName "Admin GET Organizations" -Passed $false -Details "Error: $($_.Exception.Message)" -StatusCode $statusCode
}

# Test 3: Admin CREATE Organization (Should work - admin has Create permission)
Write-Host "`n[Test 3] Admin POST (Create) Organization..." -ForegroundColor Yellow

$newOrgName = "RBAC Test Org $(Get-Date -Format 'HHmmss')"
$newOrgBody = @{
    name = $newOrgName
    description = "Created for RBAC testing"
    isActive = $true
} | ConvertTo-Json

try {
    $createHeaders = @{
        Authorization = "Bearer $adminToken"
        "Content-Type" = "application/json"
        Accept = "application/json"
    }
    
    $createResult = Invoke-RestMethod -Uri "$orgServiceUrl/api/organization-service/organizations" `
        -Method POST `
        -Headers $createHeaders `
        -Body $newOrgBody `
        -SkipCertificateCheck -ErrorAction Stop
    
    $testOrgId = $createResult.id
    Add-TestResult -TestName "Admin CREATE Organization" -Passed $true -Details "Created: $newOrgName (ID: $testOrgId)" -StatusCode 201
} catch {
    $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
    Add-TestResult -TestName "Admin CREATE Organization" -Passed $false -Details "Error: $($_.Exception.Message)" -StatusCode $statusCode
}

# Test 4: Admin UPDATE Organization (Should work - admin has Edit permission)
if ($testOrgId) {
    Write-Host "`n[Test 4] Admin PUT (Update) Organization..." -ForegroundColor Yellow
    
    # First get the organization to update
    try {
        $orgToUpdate = Invoke-RestMethod -Uri "$orgServiceUrl/api/organization-service/organizations/$testOrgId" `
            -Method GET `
            -Headers @{Authorization = "Bearer $adminToken"} `
            -SkipCertificateCheck -ErrorAction Stop
        
        $orgToUpdate.description = "Updated for RBAC testing"
        $updateBody = $orgToUpdate | ConvertTo-Json
        
        $updateResult = Invoke-RestMethod -Uri "$orgServiceUrl/api/organization-service/organizations/$testOrgId" `
            -Method PUT `
            -Headers (@{Authorization = "Bearer $adminToken"; "Content-Type" = "application/json"}) `
            -Body $updateBody `
            -SkipCertificateCheck -ErrorAction Stop
        
        Add-TestResult -TestName "Admin UPDATE Organization" -Passed $true -Details "Updated: $newOrgName" -StatusCode 200
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        Add-TestResult -TestName "Admin UPDATE Organization" -Passed $false -Details "Error: $($_.Exception.Message)" -StatusCode $statusCode
    }
}

# Test 5: Admin DELETE Organization (Should work - admin has Delete permission)
if ($testOrgId) {
    Write-Host "`n[Test 5] Admin DELETE Organization..." -ForegroundColor Yellow
    
    try {
        Invoke-RestMethod -Uri "$orgServiceUrl/api/organization-service/organizations/$testOrgId" `
            -Method DELETE `
            -Headers @{Authorization = "Bearer $adminToken"} `
            -SkipCertificateCheck -ErrorAction Stop
        
        Add-TestResult -TestName "Admin DELETE Organization" -Passed $true -Details "Deleted: $newOrgName" -StatusCode 204
    } catch {
        $statusCode = if ($_.Exception.Response) { $_.Exception.Response.StatusCode.value__ } else { 0 }
        Add-TestResult -TestName "Admin DELETE Organization" -Passed $false -Details "Error: $($_.Exception.Message)" -StatusCode $statusCode
    }
}

# Test 6: Create Test User with Limited Permissions
Write-Host "`n[Test 6] Creating Test User with Limited Permissions..." -ForegroundColor Yellow
Write-Host "⚠️  Note: User management requires UI or direct database access" -ForegroundColor Gray
Write-Host "   Skipping automated user creation for now" -ForegroundColor Gray
Write-Host "   Manual steps required:" -ForegroundColor Gray
Write-Host "   1. Create role 'LimitedUser' with only ViewOwn permission" -ForegroundColor Gray
Write-Host "   2. Create user 'testuser' with role 'LimitedUser'" -ForegroundColor Gray
Write-Host "   3. Test API calls with limited user token" -ForegroundColor Gray

# Test Summary
Write-Host "`n========================================" -ForegroundColor Cyan
Write-Host " Test Summary" -ForegroundColor Cyan
Write-Host "========================================`n" -ForegroundColor Cyan

$testResults | Format-Table -AutoSize

$passedTests = ($testResults | Where-Object { $_.Result -eq "✅ PASS" }).Count
$totalTests = $testResults.Count

Write-Host "`n📊 Results: $passedTests / $totalTests tests passed" -ForegroundColor $(if ($passedTests -eq $totalTests) { "Green" } else { "Yellow" })

if ($passedTests -eq $totalTests) {
    Write-Host "✅ All admin tests passed! RBAC is working correctly for admin user." -ForegroundColor Green
} else {
    Write-Host "⚠️  Some tests failed. Review the details above." -ForegroundColor Yellow
}

Write-Host "`n📝 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Create test user with limited permissions via Blazor UI" -ForegroundColor White
Write-Host "2. Run limited user tests (see TEST_ORGANIZATION_SERVICE.md)" -ForegroundColor White
Write-Host "3. Verify 403 errors for unauthorized operations" -ForegroundColor White
Write-Host "4. Document results in TEST_RESULTS.md" -ForegroundColor White

Write-Host "`n========================================`n" -ForegroundColor Cyan
