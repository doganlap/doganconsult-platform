# Phase 1 RBAC - Quick Testing Guide

## 🚀 Quick Start (5 Minutes)

### 1. Start Services
```powershell
cd d:\test
.\start-services.ps1
```
**Wait**: 30-60 seconds for all services to start

### 2. Access Platform
- **URL**: https://localhost:44373
- **Admin Login**: admin / 1q2w3E***

### 3. View Permissions
1. Click **Administration** (left sidebar)
2. Click **Identity Management** → **Roles**
3. Click **admin** role
4. Click **Permissions** tab
5. **Expected**: See 6 permission groups with 66 total permissions

---

## ✅ Quick Verification Checklist

### Service Authorization (API Level)
- [ ] OrganizationAppService has 6 `[Authorize]` attributes
- [ ] WorkspaceAppService has 6 `[Authorize]` attributes
- [ ] DocumentAppService has 6 `[Authorize]` attributes
- [ ] AIAppService has 1 `[Authorize]` attribute
- [ ] AuditLogAppService has 3 `[Authorize]` attributes
- [ ] UserProfileAppService has 5 `[Authorize]` attributes

**Total**: 27 method-level authorization checks

### UI Authorization (Blazor Level)
- [ ] Organizations.razor hides "Add Organization" button if no Create permission
- [ ] Workspaces.razor hides Edit/Delete buttons if no permission
- [ ] Documents.razor hides Upload/Delete buttons if no permission
- [ ] AuditLogs.razor protected by ViewAll permission
- [ ] UserProfiles.razor hides Create button if no permission
- [ ] AIChat.razor disabled if no AI permission

**Total**: 6 pages with permission checks

---

## 🧪 30-Second Smoke Test

### Test 1: Admin Can See Everything
1. Login as **admin**
2. Navigate to **/organizations**
3. **Expected**: "Add Organization" button visible
4. **Expected**: Edit/Delete buttons visible in table
5. ✅ **PASS** if all buttons visible

### Test 2: Create Limited User
1. Go to **Administration → Identity → Roles**
2. Click **New Role**
3. Name: **"Viewer"**
4. Grant ONLY: Organizations.ViewOwn
5. Save
6. Go to **Administration → Identity → Users**
7. Create user: **viewer@test.com** / **Test1234!**
8. Assign role: **Viewer**

### Test 3: Limited User Cannot Create
1. Logout
2. Login as **viewer@test.com**
3. Navigate to **/organizations**
4. **Expected**: "Add Organization" button **HIDDEN**
5. **Expected**: Edit/Delete buttons **HIDDEN**
6. ✅ **PASS** if buttons hidden

### Test 4: API Returns 403
1. Stay logged in as **viewer@test.com**
2. Open **DevTools** (F12) → **Network** tab
3. Try to navigate to **Organizations** page
4. Look for API call: `GET /api/organization-service/organizations`
5. Check if user tries any action (will trigger API call)
6. **Expected**: Some API calls may return **403 Forbidden**
7. ✅ **PASS** if 403 status codes appear

---

## 🔍 Detailed Testing Scenarios

### Scenario 1: Create Operation
**Setup**: Login as user with NO Create permission

**Test**:
1. Navigate to /organizations
2. "Add Organization" button should be **HIDDEN**
3. If you manually call API: `POST /api/organization-service/organizations`
4. **Expected**: 403 Forbidden

**Success Criteria**: ✅ Button hidden AND API returns 403

### Scenario 2: Edit Operation
**Setup**: Login as user with NO Edit permission

**Test**:
1. Navigate to /organizations
2. Edit buttons in table should be **HIDDEN**
3. If you manually call API: `PUT /api/organization-service/organizations/{id}`
4. **Expected**: 403 Forbidden

**Success Criteria**: ✅ Button hidden AND API returns 403

### Scenario 3: Delete Operation
**Setup**: Login as user with NO Delete permission

**Test**:
1. Navigate to /organizations
2. Delete buttons in table should be **HIDDEN**
3. If you manually call API: `DELETE /api/organization-service/organizations/{id}`
4. **Expected**: 403 Forbidden

**Success Criteria**: ✅ Button hidden AND API returns 403

### Scenario 4: ViewAll vs ViewOwn
**Setup**: User with ViewOwn (but not ViewAll)

**Test**:
1. Login as user
2. Navigate to /organizations
3. **Expected**: See ONLY organizations belonging to user's tenant
4. **Expected**: Cannot see organizations from other tenants

**Success Criteria**: ✅ Data filtered by tenant

### Scenario 5: Permission Grant Takes Effect
**Setup**: User with NO Create permission initially

**Test**:
1. Login as limited user
2. Verify "Add Organization" button **HIDDEN**
3. Logout
4. Login as **admin**
5. Grant Create permission to user's role
6. Logout
7. Login as limited user again
8. **Expected**: "Add Organization" button now **VISIBLE**
9. Try creating organization
10. **Expected**: 200 OK (succeeds)

**Success Criteria**: ✅ Permission change takes effect immediately after re-login

---

## 📊 Expected Results Matrix

| User Role | Organizations.Create | Organizations.Edit | Organizations.Delete | Organizations.ViewAll |
|-----------|---------------------|-------------------|---------------------|----------------------|
| **Admin** | ✅ Allowed | ✅ Allowed | ✅ Allowed | ✅ Allowed |
| **Manager** | ✅ Allowed | ✅ Allowed | ❌ Denied (403) | ✅ Allowed |
| **Viewer** | ❌ Denied (403) | ❌ Denied (403) | ❌ Denied (403) | ✅ Allowed |
| **Limited** | ❌ Denied (403) | ❌ Denied (403) | ❌ Denied (403) | ❌ Denied (403) |

---

## 🐛 Quick Troubleshooting

### Problem: Permissions not showing in admin UI
```powershell
# Restart Identity service
cd d:\test\aspnet-core\src\DoganConsult.Identity.HttpApi.Host
dotnet run
```

### Problem: User still has access after revoking permission
```
1. Force logout user
2. Clear browser cache (Ctrl+Shift+Delete)
3. Login again
4. Verify role permissions in database:
   SELECT * FROM AbpPermissionGrants WHERE RoleName = 'YourRole';
```

### Problem: 403 error even with correct permission
```
1. Check user's roles: Administration → Identity → Users → Edit User → Roles
2. Check role's permissions: Administration → Identity → Roles → Edit Role → Permissions
3. Verify tenant context (multi-tenancy isolation)
4. Check browser console for CORS errors
```

### Problem: Buttons still visible after removing permission
```
1. Hard refresh browser (Ctrl+F5)
2. Clear browser cache
3. Verify permission revocation: Administration → Identity → Roles → Permissions
4. Check Blazor component loaded new permissions (check OnInitializedAsync called)
```

---

## 📝 Quick Test Report Template

```markdown
# Phase 1 RBAC Testing Results

**Tester**: [Your Name]
**Date**: [Date]
**Duration**: [Time Taken]

## Test Results Summary
- ✅ Service Authorization: [PASS/FAIL]
- ✅ UI Authorization: [PASS/FAIL]
- ✅ Permission Grant/Revoke: [PASS/FAIL]
- ✅ Multi-Tenancy Isolation: [PASS/FAIL]

## Detailed Results

### Service Authorization
| Service | Method | Expected | Actual | Pass/Fail |
|---------|--------|----------|--------|-----------|
| Organization | CreateAsync | 403 | [Actual] | [✅/❌] |
| Organization | EditAsync | 403 | [Actual] | [✅/❌] |
| ... | ... | ... | ... | ... |

### UI Authorization
| Page | Button | Expected | Actual | Pass/Fail |
|------|--------|----------|--------|-----------|
| Organizations | Add | Hidden | [Actual] | [✅/❌] |
| Organizations | Edit | Hidden | [Actual] | [✅/❌] |
| ... | ... | ... | ... | ... |

## Issues Found
1. [Issue description]
   - **Severity**: [High/Medium/Low]
   - **Steps to reproduce**: [Steps]
   - **Expected**: [Expected behavior]
   - **Actual**: [Actual behavior]

## Screenshots
- [Attach permission management UI screenshot]
- [Attach 403 error screenshot]
- [Attach hidden buttons screenshot]

## Recommendations
- [Any recommendations for improvements]
```

---

## 🎯 Success Indicators

### ✅ Phase 1 is COMPLETE when:
- [ ] All 66 permissions visible in admin UI
- [ ] Create test user with limited permissions
- [ ] Limited user sees hidden buttons (UI-level security)
- [ ] Limited user gets 403 errors when calling APIs (service-level security)
- [ ] Admin can grant permission and user immediately gets access
- [ ] All 6 services return 403 for unauthorized operations
- [ ] Multi-tenancy isolation verified (users see only their tenant's data)
- [ ] Test report completed with no critical issues

---

## 🚀 Next Actions After Testing

1. Document any issues found
2. Fix critical bugs (if any)
3. Update this testing guide with lessons learned
4. Create test automation scripts (optional)
5. Move to **Phase 2**: Tenant Management UI

---

**Time Estimate**: 2-3 hours for comprehensive testing
**Priority**: Complete testing before moving to Phase 2
**Status**: Ready for testing ✅
