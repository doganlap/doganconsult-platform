namespace DoganConsult.Web.Permissions;

public static class WebPermissions
{
    public const string GroupName = "Web";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class Demos
    {
        public const string Default = GroupName + ".Demos";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete";
        public const string Approve = Default + ".Approve";
        public const string ManageWorkflow = Default + ".ManageWorkflow";
    }
}
