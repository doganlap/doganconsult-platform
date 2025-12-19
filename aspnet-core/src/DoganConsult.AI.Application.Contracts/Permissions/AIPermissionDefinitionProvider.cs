using DoganConsult.AI.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.AI.Permissions;

public class AIPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var aiGroup = context.AddGroup(AIPermissions.GroupName, L("Permission:AIManagement"));

        // AI Requests Permissions
        var aiRequestsPermission = aiGroup.AddPermission(
            AIPermissions.AIRequests.Default,
            L("Permission:AIRequests")
        );
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.Create, L("Permission:AIRequests.Create"));
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.ViewAll, L("Permission:AIRequests.ViewAll"));
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.ViewOwn, L("Permission:AIRequests.ViewOwn"));
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.Delete, L("Permission:AIRequests.Delete"));
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.UseAdvancedModels, L("Permission:AIRequests.UseAdvancedModels"));
        aiRequestsPermission.AddChild(AIPermissions.AIRequests.UseTools, L("Permission:AIRequests.UseTools"));

        // Agent Permissions
        var agentsPermission = aiGroup.AddPermission(
            AIPermissions.Agents.Default,
            L("Permission:Agents")
        );
        agentsPermission.AddChild(AIPermissions.Agents.AuditAgent, L("Permission:Agents.AuditAgent"));
        agentsPermission.AddChild(AIPermissions.Agents.ComplianceAgent, L("Permission:Agents.ComplianceAgent"));
        agentsPermission.AddChild(AIPermissions.Agents.GeneralAgent, L("Permission:Agents.GeneralAgent"));
        agentsPermission.AddChild(AIPermissions.Agents.CreateCustomAgent, L("Permission:Agents.CreateCustomAgent"));
        agentsPermission.AddChild(AIPermissions.Agents.ManageAgents, L("Permission:Agents.ManageAgents"));

        // Settings Permissions
        var settingsPermission = aiGroup.AddPermission(
            AIPermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(AIPermissions.Settings.ManageModels, L("Permission:Settings.ManageModels"));
        settingsPermission.AddChild(AIPermissions.Settings.ManageQuotas, L("Permission:Settings.ManageQuotas"));
        settingsPermission.AddChild(AIPermissions.Settings.ViewUsageStatistics, L("Permission:Settings.ViewUsageStatistics"));

        // Conversations Permissions
        var conversationsPermission = aiGroup.AddPermission(
            AIPermissions.Conversations.Default,
            L("Permission:Conversations")
        );
        conversationsPermission.AddChild(AIPermissions.Conversations.ViewAll, L("Permission:Conversations.ViewAll"));
        conversationsPermission.AddChild(AIPermissions.Conversations.ViewOwn, L("Permission:Conversations.ViewOwn"));
        conversationsPermission.AddChild(AIPermissions.Conversations.Delete, L("Permission:Conversations.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AIResource>(name);
    }
}
