using DoganConsult.UserProfile.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.UserProfile.Permissions;

public class UserProfilePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(UserProfilePermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(UserProfilePermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<UserProfileResource>(name);
    }
}
