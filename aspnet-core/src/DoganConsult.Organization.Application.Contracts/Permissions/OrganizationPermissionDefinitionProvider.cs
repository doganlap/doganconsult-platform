using DoganConsult.Organization.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Organization.Permissions;

public class OrganizationPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(OrganizationPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(OrganizationPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<OrganizationResource>(name);
    }
}
