using DoganConsult.Web.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Web.Permissions;

public class WebPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WebPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(WebPermissions.MyPermission1, L("Permission:MyPermission1"));

        var demosPermission = myGroup.AddPermission(WebPermissions.Demos.Default, L("Permission:Demos"));
        demosPermission.AddChild(WebPermissions.Demos.Create, L("Permission:Demos.Create"));
        demosPermission.AddChild(WebPermissions.Demos.Edit, L("Permission:Demos.Edit"));
        demosPermission.AddChild(WebPermissions.Demos.Delete, L("Permission:Demos.Delete"));
        demosPermission.AddChild(WebPermissions.Demos.Approve, L("Permission:Demos.Approve"));
        demosPermission.AddChild(WebPermissions.Demos.ManageWorkflow, L("Permission:Demos.ManageWorkflow"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WebResource>(name);
    }
}
