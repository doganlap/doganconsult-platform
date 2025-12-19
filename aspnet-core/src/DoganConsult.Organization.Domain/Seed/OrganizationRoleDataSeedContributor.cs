using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization.Domain.Shared.Permissions;
using Microsoft.AspNetCore.Identity;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Guids;

namespace DoganConsult.Organization.Domain.Seed;

public class OrganizationRoleDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IIdentityRoleRepository _roleRepository;
    private readonly IPermissionManager _permissionManager;
    private readonly IGuidGenerator _guidGenerator;

    public OrganizationRoleDataSeedContributor(
        IIdentityRoleRepository roleRepository,
        IPermissionManager permissionManager,
        IGuidGenerator guidGenerator)
    {
        _roleRepository = roleRepository;
        _permissionManager = permissionManager;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // Define roles and permission sets (Tenant-side)
        var rolePermissions = new Dictionary<string, string[]>
        {
            ["DataEntry"] = new[]
            {
                OrganizationPermissions.Platform.Dashboard,
                OrganizationPermissions.Platform.WorkCenter,

                OrganizationPermissions.Org.Workspaces,
                OrganizationPermissions.Org.Profiles,

                OrganizationPermissions.Content.Documents,
                OrganizationPermissions.Content.DocumentsCreate,

                OrganizationPermissions.AI.Chat
            },

            ["Manager"] = new[]
            {
                OrganizationPermissions.Platform.Dashboard,
                OrganizationPermissions.Platform.WorkCenter,
                OrganizationPermissions.Platform.Approvals,
                OrganizationPermissions.Platform.ApprovalsDecide,

                OrganizationPermissions.Org.Organizations,
                OrganizationPermissions.Org.Workspaces,
                OrganizationPermissions.Org.WorkspacesManage,
                OrganizationPermissions.Org.Profiles,
                OrganizationPermissions.Org.ProfilesManage,

                OrganizationPermissions.Content.Documents,
                OrganizationPermissions.Content.DocumentsCreate,
                OrganizationPermissions.Content.DocumentsEdit,
                OrganizationPermissions.Content.DocumentsApprove,

                OrganizationPermissions.AI.Chat
            },

            ["Auditor"] = new[]
            {
                OrganizationPermissions.Platform.Dashboard,
                OrganizationPermissions.Platform.Approvals, // view history

                OrganizationPermissions.Content.Documents,
                OrganizationPermissions.Content.AuditLogs,

                OrganizationPermissions.AI.Chat
            },

            ["ComplianceOfficer"] = new[]
            {
                OrganizationPermissions.Platform.Dashboard,
                OrganizationPermissions.Platform.WorkCenter,
                OrganizationPermissions.Platform.Approvals,
                OrganizationPermissions.Platform.ApprovalsDecide,

                OrganizationPermissions.Org.Workspaces,
                OrganizationPermissions.Org.Profiles,

                OrganizationPermissions.Content.Documents,
                OrganizationPermissions.Content.DocumentsApprove,

                OrganizationPermissions.Content.AuditLogs,
                OrganizationPermissions.AI.Chat
            },

            ["GrcAdmin"] = new[]
            {
                // Everything operational
                OrganizationPermissions.Platform.Dashboard,
                OrganizationPermissions.Platform.WorkCenter,
                OrganizationPermissions.Platform.Approvals,
                OrganizationPermissions.Platform.ApprovalsDecide,

                OrganizationPermissions.Org.Organizations,
                OrganizationPermissions.Org.Workspaces,
                OrganizationPermissions.Org.WorkspacesManage,
                OrganizationPermissions.Org.Profiles,
                OrganizationPermissions.Org.ProfilesManage,

                OrganizationPermissions.Content.Documents,
                OrganizationPermissions.Content.DocumentsCreate,
                OrganizationPermissions.Content.DocumentsEdit,
                OrganizationPermissions.Content.DocumentsDelete,
                OrganizationPermissions.Content.DocumentsApprove,
                OrganizationPermissions.Content.AuditLogs,

                OrganizationPermissions.AI.Chat,
                OrganizationPermissions.AI.ChatAdmin,

                // Admin
                OrganizationPermissions.Admin.Entities,
                OrganizationPermissions.Admin.Settings,
                OrganizationPermissions.Admin.IdentityRoles,
                OrganizationPermissions.Admin.IdentityUsers
            }
        };

        foreach (var (roleName, permissions) in rolePermissions)
        {
            var role = await _roleRepository.FindByNormalizedNameAsync(roleName.ToUpperInvariant());
            if (role == null)
            {
                role = new IdentityRole(_guidGenerator.Create(), roleName, context.TenantId)
                {
                    IsDefault = false,
                    IsPublic = true
                };

                await _roleRepository.InsertAsync(role);
            }

            // Grant permissions to the role
            foreach (var permission in permissions)
            {
                await _permissionManager.SetForRoleAsync(role.Name, permission, true);
            }
        }
    }
}
