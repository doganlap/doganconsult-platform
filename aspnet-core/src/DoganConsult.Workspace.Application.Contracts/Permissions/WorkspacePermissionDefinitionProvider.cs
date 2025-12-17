using DoganConsult.Workspace.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Workspace.Permissions;

public class WorkspacePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WorkspacePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(WorkspacePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WorkspaceResource>(name);
    }
}
