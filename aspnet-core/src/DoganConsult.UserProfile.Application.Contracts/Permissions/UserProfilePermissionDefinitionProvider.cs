using DoganConsult.UserProfile.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.UserProfile.Permissions;

public class UserProfilePermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var userProfileGroup = context.AddGroup(UserProfilePermissions.GroupName, L("Permission:UserProfileManagement"));

        // Profiles Permissions
        var profilesPermission = userProfileGroup.AddPermission(
            UserProfilePermissions.Profiles.Default,
            L("Permission:Profiles")
        );
        profilesPermission.AddChild(UserProfilePermissions.Profiles.Create, L("Permission:Profiles.Create"));
        profilesPermission.AddChild(UserProfilePermissions.Profiles.Edit, L("Permission:Profiles.Edit"));
        profilesPermission.AddChild(UserProfilePermissions.Profiles.Delete, L("Permission:Profiles.Delete"));
        profilesPermission.AddChild(UserProfilePermissions.Profiles.ViewAll, L("Permission:Profiles.ViewAll"));
        profilesPermission.AddChild(UserProfilePermissions.Profiles.ViewOwn, L("Permission:Profiles.ViewOwn"));
        profilesPermission.AddChild(UserProfilePermissions.Profiles.ManageAvatar, L("Permission:Profiles.ManageAvatar"));

        // Settings Permissions
        var settingsPermission = userProfileGroup.AddPermission(
            UserProfilePermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(UserProfilePermissions.Settings.Manage, L("Permission:Settings.Manage"));
        settingsPermission.AddChild(UserProfilePermissions.Settings.ManageNotifications, L("Permission:Settings.ManageNotifications"));
        settingsPermission.AddChild(UserProfilePermissions.Settings.ManagePrivacy, L("Permission:Settings.ManagePrivacy"));

        // Reports Permissions
        var reportsPermission = userProfileGroup.AddPermission(
            UserProfilePermissions.Reports.Default,
            L("Permission:Reports")
        );
        reportsPermission.AddChild(UserProfilePermissions.Reports.View, L("Permission:Reports.View"));
        reportsPermission.AddChild(UserProfilePermissions.Reports.Export, L("Permission:Reports.Export"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<UserProfileResource>(name);
    }
}
