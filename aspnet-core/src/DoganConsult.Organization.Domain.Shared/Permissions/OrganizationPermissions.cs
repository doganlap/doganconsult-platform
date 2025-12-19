namespace DoganConsult.Organization.Domain.Shared.Permissions;

public static class OrganizationPermissions
{
    public const string GroupName = "Organization";

    // Platform
    public static class Platform
    {
        public const string Default = GroupName + ".Platform";
        public const string Dashboard = Default + ".Dashboard";
        public const string WorkCenter = Default + ".WorkCenter";
        public const string Approvals = Default + ".Approvals";
        public const string ApprovalsDecide = Approvals + ".Decide";
    }

    // Organization
    public static class Org
    {
        public const string Default = GroupName + ".Org";
        public const string Organizations = Default + ".Organizations";
        public const string Workspaces = Default + ".Workspaces";
        public const string WorkspacesManage = Workspaces + ".Manage";
        public const string Profiles = Default + ".Profiles";
        public const string ProfilesManage = Profiles + ".Manage";
    }

    // Content
    public static class Content
    {
        public const string Default = GroupName + ".Content";
        public const string Documents = Default + ".Documents";
        public const string DocumentsCreate = Documents + ".Create";
        public const string DocumentsEdit = Documents + ".Edit";
        public const string DocumentsDelete = Documents + ".Delete";
        public const string DocumentsApprove = Documents + ".Approve";

        public const string AuditLogs = Default + ".AuditLogs";
    }

    // Intelligence
    public static class AI
    {
        public const string Default = GroupName + ".AI";
        public const string Chat = Default + ".Chat";
        public const string ChatAdmin = Chat + ".Admin";
    }

    // Admin
    public static class Admin
    {
        public const string Default = GroupName + ".Admin";
        public const string Entities = Default + ".Entities";
        public const string Settings = Default + ".Settings";

        public const string Identity = Default + ".Identity";
        public const string IdentityRoles = Identity + ".Roles";
        public const string IdentityUsers = Identity + ".Users";
    }

    // Demo
    public static class Demo
    {
        public const string Default = GroupName + ".Demo";
        public const string DemoProcess = Default + ".Process";
    }
}
