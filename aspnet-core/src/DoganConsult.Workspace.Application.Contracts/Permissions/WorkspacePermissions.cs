namespace DoganConsult.Workspace.Permissions;

public static class WorkspacePermissions
{
    public const string GroupName = "Workspace";

    public static class Workspaces
    {
        public const string Default = GroupName + ".Workspaces";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string ManageMembers = Default + ".ManageMembers";
        public const string Export = Default + ".Export";
    }

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Manage = Default + ".Manage";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Generate = Default + ".Generate";
    }
}
