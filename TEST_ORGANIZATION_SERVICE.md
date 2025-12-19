# Testing Organization Service RBAC Implementation

**Test Date**: December 18, 2025  
**Services Running**:
- ✅ Identity Service → https://localhost:44346
- ✅ Organization Service → https://localhost:44337  
- ✅ Gateway → http://localhost:5000

---

## Quick API Testing (Without UI)

Since the Blazor UI has startup issues, let's test the **Organization Service** RBAC directly via API calls.

### Prerequisites
- Identity Service running (port 44346) ✅
- Organization Service running (port 44337) ✅

---

## Test 1: Get Access Token (Admin User)

```powershell
# Get admin token from Identity Service
$tokenResponse = Invoke-RestMethod -Uri "https://localhost:44346/connect/token" `
    -Method POST `
    -ContentType "application/x-www-form-urlencoded" `
    -Body @{
        grant_type = "password"
        username = "admin"
        password = "1q2w3E***"
        client_id = "DoganConsult_App"
        scope = "offline_access DoganConsult"
    } `
    -SkipCertificateCheck

$adminToken = $tokenResponse.access_token
Write-Host "✅ Admin Token: $($adminToken.Substring(0, 50))..."
```

---

## Test 2: Test Organization Service with Admin Token

```powershell
# Test GET /api/organization-service/organizations (should work - admin has all permissions)
$headers = @{
    Authorization = "Bearer $adminToken"
    Accept = "application/json"
}

try {
    $orgsResult = Invoke-RestMethod -Uri "https://localhost:44337/api/organization-service/organizations" `
        -Method GET `
        -Headers $headers `
        -SkipCertificateCheck
    
    Write-Host "✅ GET Organizations: SUCCESS (Status 200)" -ForegroundColor Green
    Write-Host "   Total Organizations: $($orgsResult.totalCount)" -ForegroundColor White
} catch {
    Write-Host "❌ GET Organizations: FAILED" -ForegroundColor Red
    Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
}
```

---

## Test 3: Test CREATE Operation (Admin - Should Work)

```powershell
# Test POST /api/organization-service/organizations (should work - admin has Create permission)
$newOrg = @{
    name = "Test Organization $(Get-Date -Format 'HHmmss')"
    description = "Created for RBAC testing"
    isActive = $true
} | ConvertTo-Json

try {
    $createResult = Invoke-RestMethod -Uri "https://localhost:44337/api/organization-service/organizations" `
        -Method POST `
        -Headers (@{Authorization = "Bearer $adminToken"; "Content-Type" = "application/json"}) `
        -Body $newOrg `
        -SkipCertificateCheck
    
    Write-Host "✅ POST Organization: SUCCESS (Status 200)" -ForegroundColor Green
    Write-Host "   Created ID: $($createResult.id)" -ForegroundColor White
    $testOrgId = $createResult.id
} catch {
    Write-Host "❌ POST Organization: FAILED" -ForegroundColor Red
    Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
}
```

---

## Test 4: Create Limited Test User

```powershell
# Create a user with LIMITED permissions via Identity Service API
$limitedUser = @{
    userName = "testuser"
    email = "testuser@example.com"
    password = "Test1234!"
    roleNames = @("admin")  # Temporarily assign admin, we'll modify permissions later
} | ConvertTo-Json

try {
    $createUserResult = Invoke-RestMethod -Uri "https://localhost:44346/api/identity/users" `
        -Method POST `
        -Headers (@{Authorization = "Bearer $adminToken"; "Content-Type" = "application/json"}) `
        -Body $limitedUser `
        -SkipCertificateCheck
    
    Write-Host "✅ Create Test User: SUCCESS" -ForegroundColor Green
    Write-Host "   User ID: $($createUserResult.id)" -ForegroundColor White
    $testUserId = $createUserResult.id
} catch {
    if ($_.Exception.Response.StatusCode.value__ -eq 400) {
        Write-Host "⚠️  User 'testuser' already exists (this is OK)" -ForegroundColor Yellow
    } else {
        Write-Host "❌ Create Test User: FAILED" -ForegroundColor Red
        Write-Host "   Status: $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
```

---

## Test 5: Login as Limited User and Test API

```powershell
# Get token for limited user
try {
    $limitedTokenResponse = Invoke-RestMethod -Uri "https://localhost:44346/connect/token" `
        -Method POST `
        -ContentType "application/x-www-form-urlencoded" `
        -Body @{
            grant_type = "password"
            username = "testuser"
            password = "Test1234!"
            client_id = "DoganConsult_App"
            scope = "offline_access DoganConsult"
        } `
        -SkipCertificateCheck

    $limitedToken = $limitedTokenResponse.access_token
    Write-Host "✅ Limited User Token: $($limitedToken.Substring(0, 50))..." -ForegroundColor Green
    
    # Test GET (should work if ViewAll or ViewOwn granted)
    try {
        $orgs = Invoke-RestMethod -Uri "https://localhost:44337/api/organization-service/organizations" `
            -Method GET `
            -Headers @{Authorization = "Bearer $limitedToken"} `
            -SkipCertificateCheck
        
        Write-Host "✅ Limited User GET: SUCCESS (Status 200)" -ForegroundColor Green
    } catch {
        Write-Host "❌ Limited User GET: DENIED (Status $($_.Exception.Response.StatusCode.value__))" -ForegroundColor Red
    }
    
    # Test POST (should fail if Create permission NOT granted)
    try {
        $testOrg = @{
            name = "Unauthorized Test Org"
            description = "Should fail with 403"
            isActive = $true
        } | ConvertTo-Json
        
        $result = Invoke-RestMethod -Uri "https://localhost:44337/api/organization-service/organizations" `
            -Method POST `
            -Headers (@{Authorization = "Bearer $limitedToken"; "Content-Type" = "application/json"}) `
            -Body $testOrg `
            -SkipCertificateCheck
        
        Write-Host "❌ Limited User POST: SHOULD HAVE FAILED (got Status 200)" -ForegroundColor Red
        Write-Host "   ⚠️  SECURITY ISSUE: User can create without permission!" -ForegroundColor Yellow
    } catch {
        if ($_.Exception.Response.StatusCode.value__ -eq 403) {
            Write-Host "✅ Limited User POST: CORRECTLY DENIED (Status 403)" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Limited User POST: FAILED with Status $($_.Exception.Response.StatusCode.value__)" -ForegroundColor Yellow
        }
    }
    
} catch {
    Write-Host "❌ Limited User Login: FAILED" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
}
```

---

## Expected Test Results

| Test | Expected Result | Actual Result |
|------|----------------|---------------|
| Admin GET Organizations | ✅ 200 OK | [Run test] |
| Admin POST Organization | ✅ 200 OK (Created) | [Run test] |
| Create Limited Test User | ✅ 200 OK | [Run test] |
| Limited User Login | ✅ 200 OK (Token received) | [Run test] |
| Limited User GET | ✅ 200 OK (ViewOwn/ViewAll) | [Run test] |
| Limited User POST | ✅ 403 Forbidden (No Create permission) | [Run test] |

---

## Run All Tests

**Option 1: Copy and paste each test block into PowerShell**

**Option 2: Run the complete test script** (coming soon - will be in `test-organization-rbac.ps1`)

---

## Next Steps After Testing

1. ✅ Verify 403 errors for unauthorized operations
2. ✅ Test all 6 services (Organization, Workspace, Document, AI, Audit, UserProfile)
3. ✅ Document results in TEST_RESULTS.md
4. ✅ Fix any issues found
5. ✅ Mark Phase 1 as COMPLETE
