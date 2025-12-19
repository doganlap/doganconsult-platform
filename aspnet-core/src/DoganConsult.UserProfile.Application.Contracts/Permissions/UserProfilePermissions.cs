namespace DoganConsult.UserProfile.Permissions;

public static class UserProfilePermissions
{
    public const string GroupName = "UserProfile";

    public static class Profiles
    {
        public const string Default = GroupName + ".Profiles";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string ManageAvatar = Default + ".ManageAvatar";
    }

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Manage = Default + ".Manage";
        public const string ManageNotifications = Default + ".ManageNotifications";
        public const string ManagePrivacy = Default + ".ManagePrivacy";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Export = Default + ".Export";
    }
}
