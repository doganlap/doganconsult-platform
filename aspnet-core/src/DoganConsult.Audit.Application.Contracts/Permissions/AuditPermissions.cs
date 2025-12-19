namespace DoganConsult.Audit.Permissions;

public static class AuditPermissions
{
    public const string GroupName = "Audit";

    public static class AuditLogs
    {
        public const string Default = GroupName + ".AuditLogs";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string Export = Default + ".Export";
        public const string Delete = Default + ".Delete";
    }

    public static class Approvals
    {
        public const string Default = GroupName + ".Approvals";
        public const string Create = Default + ".Create";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
        public const string Cancel = Default + ".Cancel";
        public const string Reassign = Default + ".Reassign";
        public const string Delete = Default + ".Delete";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
    }

    public static class Reports
    {
        public const string Default = GroupName + ".Reports";
        public const string View = Default + ".View";
        public const string Generate = Default + ".Generate";
        public const string Export = Default + ".Export";
    }

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string Manage = Default + ".Manage";
    }
}
