using DoganConsult.Workspace.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Workspace.Permissions;

public class WorkspacePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var workspaceGroup = context.AddGroup(WorkspacePermissions.GroupName, L("Permission:WorkspaceManagement"));

        // Workspace Permissions
        var workspacesPermission = workspaceGroup.AddPermission(
            WorkspacePermissions.Workspaces.Default,
            L("Permission:Workspaces")
        );
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.Create, L("Permission:Workspaces.Create"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.Edit, L("Permission:Workspaces.Edit"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.Delete, L("Permission:Workspaces.Delete"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.ViewAll, L("Permission:Workspaces.ViewAll"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.ViewOwn, L("Permission:Workspaces.ViewOwn"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.ManageMembers, L("Permission:Workspaces.ManageMembers"));
        workspacesPermission.AddChild(WorkspacePermissions.Workspaces.Export, L("Permission:Workspaces.Export"));

        // Settings Permissions
        var settingsPermission = workspaceGroup.AddPermission(
            WorkspacePermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(WorkspacePermissions.Settings.Manage, L("Permission:Settings.Manage"));

        // Reports Permissions
        var reportsPermission = workspaceGroup.AddPermission(
            WorkspacePermissions.Reports.Default,
            L("Permission:Reports")
        );
        reportsPermission.AddChild(WorkspacePermissions.Reports.View, L("Permission:Reports.View"));
        reportsPermission.AddChild(WorkspacePermissions.Reports.Generate, L("Permission:Reports.Generate"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkspaceResource>(name);
    }
}
