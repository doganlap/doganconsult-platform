namespace DoganConsult.Audit.Permissions;

public static class AuditPermissions
{
    public const string GroupName = "Audit";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Approvals
    {
        public const string Default = GroupName + ".Approvals";
        public const string Create = Default + ".Create";
        public const string Approve = Default + ".Approve";
        public const string Reject = Default + ".Reject";
        public const string Cancel = Default + ".Cancel";
        public const string Reassign = Default + ".Reassign";
        public const string Delete = Default + ".Delete";
    }
}
