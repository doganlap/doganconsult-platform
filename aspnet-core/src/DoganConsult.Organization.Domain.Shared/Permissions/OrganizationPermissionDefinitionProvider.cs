using DoganConsult.Organization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Organization.Domain.Shared.Permissions;

public class OrganizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(OrganizationPermissions.GroupName, L("Permission:Organization"));

        // Platform
        var platform = group.AddPermission(OrganizationPermissions.Platform.Default, L("Permission:Platform"));
        platform.AddChild(OrganizationPermissions.Platform.Dashboard, L("Permission:Platform.Dashboard"));
        platform.AddChild(OrganizationPermissions.Platform.WorkCenter, L("Permission:Platform.WorkCenter"));
        var approvals = platform.AddChild(OrganizationPermissions.Platform.Approvals, L("Permission:Platform.Approvals"));
        approvals.AddChild(OrganizationPermissions.Platform.ApprovalsDecide, L("Permission:Platform.Approvals.Decide"));

        // Organization
        var org = group.AddPermission(OrganizationPermissions.Org.Default, L("Permission:Org"));
        org.AddChild(OrganizationPermissions.Org.Organizations, L("Permission:Org.Organizations"));
        var workspaces = org.AddChild(OrganizationPermissions.Org.Workspaces, L("Permission:Org.Workspaces"));
        workspaces.AddChild(OrganizationPermissions.Org.WorkspacesManage, L("Permission:Org.Workspaces.Manage"));
        var profiles = org.AddChild(OrganizationPermissions.Org.Profiles, L("Permission:Org.Profiles"));
        profiles.AddChild(OrganizationPermissions.Org.ProfilesManage, L("Permission:Org.Profiles.Manage"));

        // Content
        var content = group.AddPermission(OrganizationPermissions.Content.Default, L("Permission:Content"));
        var docs = content.AddChild(OrganizationPermissions.Content.Documents, L("Permission:Content.Documents"));
        docs.AddChild(OrganizationPermissions.Content.DocumentsCreate, L("Permission:Content.Documents.Create"));
        docs.AddChild(OrganizationPermissions.Content.DocumentsEdit, L("Permission:Content.Documents.Edit"));
        docs.AddChild(OrganizationPermissions.Content.DocumentsDelete, L("Permission:Content.Documents.Delete"));
        docs.AddChild(OrganizationPermissions.Content.DocumentsApprove, L("Permission:Content.Documents.Approve"));
        content.AddChild(OrganizationPermissions.Content.AuditLogs, L("Permission:Content.AuditLogs"));

        // AI
        var ai = group.AddPermission(OrganizationPermissions.AI.Default, L("Permission:AI"));
        var chat = ai.AddChild(OrganizationPermissions.AI.Chat, L("Permission:AI.Chat"));
        chat.AddChild(OrganizationPermissions.AI.ChatAdmin, L("Permission:AI.Chat.Admin"));

        // Admin
        var admin = group.AddPermission(OrganizationPermissions.Admin.Default, L("Permission:Admin"));
        admin.AddChild(OrganizationPermissions.Admin.Entities, L("Permission:Admin.Entities"));
        admin.AddChild(OrganizationPermissions.Admin.Settings, L("Permission:Admin.Settings"));

        var identity = admin.AddChild(OrganizationPermissions.Admin.Identity, L("Permission:Admin.Identity"));
        identity.AddChild(OrganizationPermissions.Admin.IdentityRoles, L("Permission:Admin.Identity.Roles"));
        identity.AddChild(OrganizationPermissions.Admin.IdentityUsers, L("Permission:Admin.Identity.Users"));

        // Demo
        var demo = group.AddPermission(OrganizationPermissions.Demo.Default, L("Permission:Demo"));
        demo.AddChild(OrganizationPermissions.Demo.DemoProcess, L("Permission:Demo.Process"));
    }

    private static LocalizableString L(string name)
        => LocalizableString.Create<OrganizationResource>(name);
}
