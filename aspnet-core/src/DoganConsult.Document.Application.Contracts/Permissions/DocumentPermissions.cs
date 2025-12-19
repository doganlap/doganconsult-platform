namespace DoganConsult.Document.Permissions;

public static class DocumentPermissions
{
    public const string GroupName = "Document";

    public static class Documents
    {
        public const string Default = GroupName + ".Documents";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string Download = Default + ".Download";
        public const string Upload = Default + ".Upload";
        public const string Share = Default + ".Share";
        public const string Archive = Default + ".Archive";
    }

    public static class Folders
    {
        public const string Default = GroupName + ".Folders";
        public const string Create = Default + ".Create";
        public const string Manage = Default + ".Manage";
        public const string Delete = Default + ".Delete";
    }

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Manage = Default + ".Manage";
    }
}
