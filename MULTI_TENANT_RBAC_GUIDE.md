# Multi-Tenant & Role-Based Access Control (RBAC) Guide
**DoganConsult Platform - Complete Access Control System**

## ✅ What You Already Have (Built-In)

Your platform has a **comprehensive multi-tenant architecture with role-based access control** already implemented through ABP Framework. Here's what's configured:

### 1. Multi-Tenancy (ENABLED on All Services)

**Status**: ✅ **ACTIVE** (`MultiTenancyConsts.IsEnabled = true`)

Each service has multi-tenancy enabled:
- ✅ Identity Service
- ✅ Organization Service  
- ✅ Workspace Service
- ✅ UserProfile Service
- ✅ Document Service
- ✅ AI Service
- ✅ Audit Service
- ✅ Web Blazor UI

**What This Means**:
- Each tenant (organization) has **completely isolated data**
- Users in Tenant A **cannot see** data from Tenant B
- Each tenant has **separate database schemas** (via `TenantId` filtering)
- Support for both **Host** and **Tenant** users

---

## 🏢 Multi-Tenancy Architecture

### Tenant Isolation Levels

```
┌─────────────────────────────────────────────────┐
│                   HOST LEVEL                    │
│  (Super Admin - Manages All Tenants)           │
├─────────────────────────────────────────────────┤
│                                                 │
│  ┌───────────────┐  ┌───────────────┐         │
│  │   TENANT 1    │  │   TENANT 2    │         │
│  │  (Acme Corp)  │  │ (GlobalTech)  │         │
│  ├───────────────┤  ├───────────────┤         │
│  │ Organizations │  │ Organizations │         │
│  │ Users         │  │ Users         │         │
│  │ Workspaces    │  │ Workspaces    │         │
│  │ Documents     │  │ Documents     │         │
│  │ Roles         │  │ Roles         │         │
│  │ Permissions   │  │ Permissions   │         │
│  └───────────────┘  └───────────────┘         │
│                                                 │
└─────────────────────────────────────────────────┘
```

### Database Isolation

**Each entity has `TenantId` column**:
```sql
-- Organizations table
CREATE TABLE Organizations (
    Id uuid PRIMARY KEY,
    TenantId uuid,  -- Automatically filters per tenant
    Name varchar(255),
    ...
);

-- Workspaces table
CREATE TABLE Workspaces (
    Id uuid PRIMARY KEY,
    TenantId uuid,  -- Isolated per tenant
    Name varchar(255),
    ...
);
```

**ABP Framework automatically**:
- Adds `WHERE TenantId = @CurrentTenantId` to all queries
- Prevents cross-tenant data access
- Filters navigation properties

---

## 👥 Role-Based Access Control (RBAC)

### 1. Built-In Roles

ABP provides default roles (managed via Identity service):

```
🔐 Host Roles (Manage Platform)
├── admin          - Full platform access
└── host-manager   - Manage tenants, global settings

🏢 Tenant Roles (Per Organization)
├── admin          - Full tenant access
├── manager        - Manage users, view all data
├── user           - Standard user access
└── viewer         - Read-only access
```

### 2. Permission System (Per Service)

Each service has its own permission definitions:

#### **Organization Service Permissions**
```csharp
// File: OrganizationPermissionDefinitionProvider.cs
public static class OrganizationPermissions
{
    public const string GroupName = "Organization";

    // Example permissions to add:
    public static class Organizations
    {
        public const string Default = GroupName + ".Organizations";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
```

#### **Workspace Service Permissions**
```csharp
// File: WorkspacePermissionDefinitionProvider.cs
public static class WorkspacePermissions
{
    public const string GroupName = "Workspace";
    
    public static class Workspaces
    {
        public const string Default = GroupName + ".Workspaces";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
    }
}
```

#### **Document Service Permissions**
```csharp
// File: DocumentPermissionDefinitionProvider.cs
public static class DocumentPermissions
{
    public const string GroupName = "Document";
    
    public static class Documents
    {
        public const string Default = GroupName + ".Documents";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Download = Default + ".Download";
    }
}
```

#### **AI Service Permissions**
```csharp
// File: AIPermissionDefinitionProvider.cs
public static class AIPermissions
{
    public const string GroupName = "AI";
    
    public static class Chat
    {
        public const string Default = GroupName + ".Chat";
        public const string Create = Default + ".Create";
        public const string ViewHistory = Default + ".ViewHistory";
    }
}
```

#### **Audit Service Permissions**
```csharp
// File: AuditPermissionDefinitionProvider.cs
public static class AuditPermissions
{
    public const string GroupName = "Audit";
    
    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string View = Default + ".View";
    }
    
    public static class Approvals
    {
        public const string Default = GroupName + ".Approvals";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
    }
}
```

---

## 🎯 Feature Management (Module-Level Access)

### Current Feature Definitions

**File**: `DoganConsult.Workspace.Domain/Features/DgFeatureDefinitionProvider.cs`

```csharp
public static class DgFeatures
{
    public const string GroupName = "DG";
    
    // Module features (enabled per tenant)
    public static class Modules
    {
        public const string Sbg = GroupName + ".Modules.Sbg";
        public const string ShahinGrc = GroupName + ".Modules.ShahinGrc";
        public const string NextErp = GroupName + ".Modules.NextErp";
    }
    
    // Sub-features for granular control
    public static class SubFeatures
    {
        public static class ShahinGrc
        {
            public const string Risk = Modules.ShahinGrc + ".Risk";
            public const string Controls = Modules.ShahinGrc + ".Controls";
        }
        
        public static class Sbg
        {
            public const string Procurement = Modules.Sbg + ".Procurement";
            public const string Contracts = Modules.Sbg + ".Contracts";
        }
    }
}
```

**What This Enables**:
- ✅ Tenant 1 can have "ShahinGrc" module enabled
- ✅ Tenant 2 can have "NextErp" module enabled  
- ✅ Tenant 3 can have ALL modules enabled
- ✅ Each tenant pays only for enabled features

---

## 🛠️ How to Implement Complete RBAC

### Step 1: Define Permissions for Each Service

Let's add comprehensive permissions to the **Organization Service** as an example:

**File**: `DoganConsult.Organization.Application.Contracts/Permissions/OrganizationPermissions.cs`

```csharp
namespace DoganConsult.Organization.Permissions;

public static class OrganizationPermissions
{
    public const string GroupName = "Organization";

    // Organizations Module
    public static class Organizations
    {
        public const string Default = GroupName + ".Organizations";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ViewAll = Default + ".ViewAll"; // Can see all orgs
        public const string ViewOwn = Default + ".ViewOwn"; // Can see only assigned orgs
    }
    
    // Organization Settings
    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Manage = Default + ".Manage";
    }
    
    // Organization Members
    public static class Members
    {
        public const string Default = GroupName + ".Members";
        public const string Add = Default + ".Add";
        public const string Remove = Default + ".Remove";
        public const string ManageRoles = Default + ".ManageRoles";
    }
}
```

**File**: `DoganConsult.Organization.Application.Contracts/Permissions/OrganizationPermissionDefinitionProvider.cs`

```csharp
using DoganConsult.Organization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Organization.Permissions;

public class OrganizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var organizationGroup = context.AddGroup(
            OrganizationPermissions.GroupName,
            L("Permission:Organization")
        );

        // Organizations permissions
        var organizationsPermission = organizationGroup.AddPermission(
            OrganizationPermissions.Organizations.Default,
            L("Permission:Organizations")
        );
        organizationsPermission.AddChild(
            OrganizationPermissions.Organizations.Create,
            L("Permission:Organizations.Create")
        );
        organizationsPermission.AddChild(
            OrganizationPermissions.Organizations.Edit,
            L("Permission:Organizations.Edit")
        );
        organizationsPermission.AddChild(
            OrganizationPermissions.Organizations.Delete,
            L("Permission:Organizations.Delete")
        );
        organizationsPermission.AddChild(
            OrganizationPermissions.Organizations.ViewAll,
            L("Permission:Organizations.ViewAll")
        );
        organizationsPermission.AddChild(
            OrganizationPermissions.Organizations.ViewOwn,
            L("Permission:Organizations.ViewOwn")
        );

        // Settings permissions
        var settingsPermission = organizationGroup.AddPermission(
            OrganizationPermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(
            OrganizationPermissions.Settings.Manage,
            L("Permission:Settings.Manage")
        );

        // Members permissions
        var membersPermission = organizationGroup.AddPermission(
            OrganizationPermissions.Members.Default,
            L("Permission:Members")
        );
        membersPermission.AddChild(
            OrganizationPermissions.Members.Add,
            L("Permission:Members.Add")
        );
        membersPermission.AddChild(
            OrganizationPermissions.Members.Remove,
            L("Permission:Members.Remove")
        );
        membersPermission.AddChild(
            OrganizationPermissions.Members.ManageRoles,
            L("Permission:Members.ManageRoles")
        );
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OrganizationResource>(name);
    }
}
```

### Step 2: Apply Permissions in Services

**File**: `DoganConsult.Organization.Application/Organizations/OrganizationAppService.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Organization.Organizations;

[Authorize] // Require authentication
public class OrganizationAppService : ApplicationService, IOrganizationAppService
{
    private readonly IRepository<Organization, Guid> _organizationRepository;

    public OrganizationAppService(IRepository<Organization, Guid> organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }

    // Anyone authenticated can view their own organizations
    [Authorize(OrganizationPermissions.Organizations.ViewOwn)]
    public async Task<PagedResultDto<OrganizationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        // ABP automatically filters by TenantId
        var queryable = await _organizationRepository.GetQueryableAsync();
        
        // If user doesn't have ViewAll permission, filter by assigned orgs
        if (!await AuthorizationService.IsGrantedAsync(OrganizationPermissions.Organizations.ViewAll))
        {
            // Filter to only user's assigned organizations
            var userId = CurrentUser.Id.Value;
            queryable = queryable.Where(o => o.AssignedUserIds.Contains(userId));
        }
        
        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(o => o.Name)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<OrganizationDto>(
            totalCount,
            ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(items)
        );
    }

    // Only users with Create permission can create
    [Authorize(OrganizationPermissions.Organizations.Create)]
    public async Task<OrganizationDto> CreateAsync(CreateOrganizationDto input)
    {
        var organization = ObjectMapper.Map<CreateOrganizationDto, Organization>(input);
        await _organizationRepository.InsertAsync(organization);
        return ObjectMapper.Map<Organization, OrganizationDto>(organization);
    }

    // Only users with Edit permission can update
    [Authorize(OrganizationPermissions.Organizations.Edit)]
    public async Task<OrganizationDto> UpdateAsync(Guid id, UpdateOrganizationDto input)
    {
        var organization = await _organizationRepository.GetAsync(id);
        ObjectMapper.Map(input, organization);
        await _organizationRepository.UpdateAsync(organization);
        return ObjectMapper.Map<Organization, OrganizationDto>(organization);
    }

    // Only users with Delete permission can delete
    [Authorize(OrganizationPermissions.Organizations.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _organizationRepository.DeleteAsync(id);
    }

    // Only admins can manage members
    [Authorize(OrganizationPermissions.Members.ManageRoles)]
    public async Task AssignRoleAsync(Guid organizationId, Guid userId, string roleName)
    {
        // Implementation for role assignment
    }
}
```

### Step 3: Check Permissions in Blazor UI

**File**: `DoganConsult.Web.Blazor/Pages/Organizations.razor`

```razor
@page "/organizations"
@layout PlatformLayout
@using DoganConsult.Organization.Permissions
@using Microsoft.AspNetCore.Authorization
@inject IAuthorizationService AuthorizationService

<PageTitle>Organizations</PageTitle>

<Card>
    <CardHeader>
        <Row Class="align-items-center">
            <Column ColumnSize="ColumnSize.IsAuto">
                <h3>📊 Organizations</h3>
            </Column>
            <Column ColumnSize="ColumnSize.IsAuto" Class="ms-auto">
                @if (CanCreate)
                {
                    <Button Color="Color.Primary" Clicked="OpenCreateModalAsync">
                        <Icon Name="IconName.Add" /> New Organization
                    </Button>
                }
            </Column>
        </Row>
    </CardHeader>
    <CardBody>
        <DataGrid TItem="OrganizationDto"
                  Data="Organizations"
                  ReadData="OnDataGridReadAsync"
                  TotalItems="TotalCount"
                  ShowPager="true"
                  PageSize="10">
            <DataGridColumns>
                <DataGridColumn Field="@nameof(OrganizationDto.Name)" Caption="Name" />
                <DataGridColumn Field="@nameof(OrganizationDto.Type)" Caption="Type" />
                <DataGridColumn Caption="Actions" Width="150px">
                    <DisplayTemplate>
                        @if (CanEdit)
                        {
                            <Button Size="Size.Small" Color="Color.Warning" 
                                    Clicked="() => OpenEditModalAsync(context)">
                                <Icon Name="IconName.Edit" />
                            </Button>
                        }
                        @if (CanDelete)
                        {
                            <Button Size="Size.Small" Color="Color.Danger" 
                                    Clicked="() => DeleteAsync(context)">
                                <Icon Name="IconName.Delete" />
                            </Button>
                        }
                    </DisplayTemplate>
                </DataGridColumn>
            </DataGridColumns>
        </DataGrid>
    </CardBody>
</Card>

@code {
    private bool CanCreate { get; set; }
    private bool CanEdit { get; set; }
    private bool CanDelete { get; set; }
    private List<OrganizationDto> Organizations { get; set; } = new();
    private int TotalCount { get; set; }

    protected override async Task OnInitializedAsync()
    {
        // Check permissions
        CanCreate = await AuthorizationService.IsGrantedAsync(
            OrganizationPermissions.Organizations.Create
        );
        CanEdit = await AuthorizationService.IsGrantedAsync(
            OrganizationPermissions.Organizations.Edit
        );
        CanDelete = await AuthorizationService.IsGrantedAsync(
            OrganizationPermissions.Organizations.Delete
        );
    }

    private async Task OnDataGridReadAsync(DataGridReadDataEventArgs<OrganizationDto> e)
    {
        // Load data from service
        var result = await OrganizationAppService.GetListAsync(
            new PagedAndSortedResultRequestDto
            {
                SkipCount = e.Page * e.PageSize,
                MaxResultCount = e.PageSize
            }
        );
        
        Organizations = result.Items.ToList();
        TotalCount = (int)result.TotalCount;
        
        await InvokeAsync(StateHasChanged);
    }
}
```

---

## 🔐 Organization-Level Access Control

### Scenario: Users Can Only See Their Assigned Organizations

**Entity**: Add `AssignedUserIds` to Organization entity

```csharp
// File: DoganConsult.Organization.Domain/Organizations/Organization.cs
public class Organization : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    public Guid? TenantId { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    
    // NEW: Track which users can access this organization
    public List<Guid> AssignedUserIds { get; set; } = new();
    
    // NEW: Organization-specific roles
    public Dictionary<Guid, List<string>> UserRoles { get; set; } = new();
    // Key: UserId, Value: List of role names like ["Manager", "Editor"]
}
```

**Service**: Filter by assigned organizations

```csharp
public async Task<List<OrganizationDto>> GetMyOrganizationsAsync()
{
    var userId = CurrentUser.Id.Value;
    var queryable = await _organizationRepository.GetQueryableAsync();
    
    // Filter to only assigned organizations
    queryable = queryable.Where(o => o.AssignedUserIds.Contains(userId));
    
    var organizations = await AsyncExecuter.ToListAsync(queryable);
    return ObjectMapper.Map<List<Organization>, List<OrganizationDto>>(organizations);
}
```

---

## 🎭 Complete Access Control Matrix

### By Role

| Role          | Organizations | Workspaces | Documents | AI Chat | Audit Logs | Approvals |
|---------------|--------------|------------|-----------|---------|------------|-----------|
| **Host Admin**    | Full         | Full       | Full      | Full    | Full       | Full      |
| **Tenant Admin**  | Full         | Full       | Full      | Full    | Full       | Full      |
| **Manager**       | View All, Edit Own | Create, Edit, Delete | View All, Upload | Use | View Own | Approve |
| **User**          | View Assigned | Create Own, Edit Own | View Own, Upload | Use | View Own | Request |
| **Viewer**        | View Assigned | View Assigned | View Only | - | View Own | View Own |

### By Tenant & Organization

```
Tenant: Acme Corp (TenantId: abc-123)
├── Organization: Sales Department (OrgId: org-001)
│   ├── User: john@acme.com (Manager)
│   │   ✅ Can view/edit all workspaces in Sales Dept
│   │   ✅ Can approve documents for Sales Dept
│   │   ❌ Cannot see Marketing Dept data
│   └── User: jane@acme.com (User)
│       ✅ Can view own workspaces in Sales Dept
│       ❌ Cannot edit other users' workspaces
│
└── Organization: Marketing Department (OrgId: org-002)
    ├── User: bob@acme.com (Manager)
    │   ✅ Can view/edit all workspaces in Marketing Dept
    │   ❌ Cannot see Sales Dept data
    └── User: alice@acme.com (User)
        ✅ Can view own workspaces in Marketing Dept
```

---

## 📋 Implementation Checklist

### ✅ Already Implemented (Built-In)
- [x] Multi-tenancy enabled on all services
- [x] Tenant data isolation (TenantId filtering)
- [x] Role management (ABP Identity)
- [x] Permission definitions (per service)
- [x] Feature management (module-level access)
- [x] User authentication (OpenIddict)
- [x] Authorization middleware

### 🔨 To Implement (Custom RBAC)

#### 1. Define Permissions for Each Service
- [ ] Organization Service - Add all permissions
- [ ] Workspace Service - Add all permissions
- [ ] Document Service - Add all permissions
- [ ] AI Service - Add all permissions
- [ ] Audit Service - Add all permissions
- [ ] UserProfile Service - Add all permissions

#### 2. Apply Authorization Attributes
- [ ] Add `[Authorize(Permission)]` to all AppService methods
- [ ] Check permissions in Blazor pages
- [ ] Hide UI elements based on permissions

#### 3. Add Organization-Level Access
- [ ] Add `AssignedUserIds` to Organization entity
- [ ] Create OrganizationMember entity
- [ ] Implement organization assignment logic
- [ ] Filter data by assigned organizations

#### 4. Add Localization Keys
- [ ] Add permission display names to en.json
- [ ] Add permission display names to ar.json

#### 5. Add UI for Permission Management
- [ ] Create Role Management page
- [ ] Create Permission Assignment page
- [ ] Create Organization Assignment page

---

## 🧪 Testing Access Control

### Test Cases

**1. Tenant Isolation**
```csharp
// Login as Tenant A admin
var tenantAOrgs = await OrganizationService.GetListAsync();
// Should return only Tenant A organizations

// Login as Tenant B admin
var tenantBOrgs = await OrganizationService.GetListAsync();
// Should return only Tenant B organizations
// Tenant A data should be completely invisible
```

**2. Role-Based Access**
```csharp
// Login as Manager
var canCreate = await AuthorizationService.IsGrantedAsync(
    OrganizationPermissions.Organizations.Create
);
// Should return true

// Login as Viewer
var canCreate = await AuthorizationService.IsGrantedAsync(
    OrganizationPermissions.Organizations.Create
);
// Should return false
```

**3. Organization-Based Access**
```csharp
// Login as user assigned to Org A only
var myOrgs = await OrganizationService.GetMyOrganizationsAsync();
// Should return only Org A
// Org B should not be visible even in same tenant
```

---

## 🚀 Quick Start: Enable Full RBAC

### Step-by-Step Commands

```powershell
# 1. Define all permissions
# Edit each *PermissionDefinitionProvider.cs file
# Add permissions as shown in Step 1 above

# 2. Apply migrations (if entity changes needed)
cd aspnet-core
.\create-migrations.ps1
.\run-migrations.ps1

# 3. Rebuild solution
dotnet build DoganConsult.Platform.sln -c Release

# 4. Restart all services
.\start-services.ps1

# 5. Test in UI
# Navigate to https://localhost:44373/identity/roles
# Click on "admin" role
# Click "Permissions" tab
# You should see all defined permissions organized by service
```

---

## 📚 ABP Framework Authorization Documentation

For detailed ABP authorization features:
- https://docs.abp.io/en/abp/latest/Authorization
- https://docs.abp.io/en/abp/latest/Multi-Tenancy
- https://docs.abp.io/en/abp/latest/Features

---

## 🆘 Common Scenarios

### Scenario 1: Add "Department Manager" Role
```csharp
// 1. Create role via Identity service
var role = await IdentityRoleRepository.InsertAsync(
    new IdentityRole(GuidGenerator.Create(), "DepartmentManager", tenantId)
);

// 2. Assign permissions
await PermissionGrantRepository.InsertAsync(
    new PermissionGrant(
        GuidGenerator.Create(),
        OrganizationPermissions.Organizations.ViewAll,
        "R",  // Provider: Role
        role.Name,
        tenantId
    )
);
```

### Scenario 2: Check If User Can Approve Documents
```csharp
// In Blazor component
@if (await AuthorizationService.IsGrantedAsync(AuditPermissions.Approvals.Approve))
{
    <Button Color="Color.Success" Clicked="ApproveAsync">
        Approve
    </Button>
}
```

### Scenario 3: Restrict Feature by Tenant
```csharp
// Check if tenant has ShahinGrc module enabled
if (await FeatureChecker.IsEnabledAsync(DgFeatures.Modules.ShahinGrc))
{
    // Show ShahinGrc menu and features
}
else
{
    // Hide or show upgrade prompt
}
```

---

**End of Multi-Tenant & RBAC Guide**
