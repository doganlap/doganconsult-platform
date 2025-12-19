namespace DoganConsult.AI.Permissions;

public static class AIPermissions
{
    public const string GroupName = "AI";

    public static class AIRequests
    {
        public const string Default = GroupName + ".AIRequests";
        public const string Create = Default + ".Create";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string Delete = Default + ".Delete";
        public const string UseAdvancedModels = Default + ".UseAdvancedModels";
        public const string UseTools = Default + ".UseTools";
    }

    public static class Agents
    {
        public const string Default = GroupName + ".Agents";
        public const string AuditAgent = Default + ".AuditAgent";
        public const string ComplianceAgent = Default + ".ComplianceAgent";
        public const string GeneralAgent = Default + ".GeneralAgent";
        public const string CreateCustomAgent = Default + ".CreateCustomAgent";
        public const string ManageAgents = Default + ".ManageAgents";
    }

    public static class Settings
    {
        public const string Default = GroupName + ".Settings";
        public const string ManageModels = Default + ".ManageModels";
        public const string ManageQuotas = Default + ".ManageQuotas";
        public const string ViewUsageStatistics = Default + ".ViewUsageStatistics";
    }

    public static class Conversations
    {
        public const string Default = GroupName + ".Conversations";
        public const string ViewAll = Default + ".ViewAll";
        public const string ViewOwn = Default + ".ViewOwn";
        public const string Delete = Default + ".Delete";
    }
}
