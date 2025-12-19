using DoganConsult.Audit.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Audit.Permissions;

public class AuditPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var auditGroup = context.AddGroup(AuditPermissions.GroupName, L("Permission:AuditManagement"));

        // Audit Logs Permissions
        var auditLogsPermission = auditGroup.AddPermission(
            AuditPermissions.AuditLogs.Default,
            L("Permission:AuditLogs")
        );
        auditLogsPermission.AddChild(AuditPermissions.AuditLogs.ViewAll, L("Permission:AuditLogs.ViewAll"));
        auditLogsPermission.AddChild(AuditPermissions.AuditLogs.ViewOwn, L("Permission:AuditLogs.ViewOwn"));
        auditLogsPermission.AddChild(AuditPermissions.AuditLogs.Export, L("Permission:AuditLogs.Export"));
        auditLogsPermission.AddChild(AuditPermissions.AuditLogs.Delete, L("Permission:AuditLogs.Delete"));

        // Approvals Permissions
        var approvalsPermission = auditGroup.AddPermission(
            AuditPermissions.Approvals.Default,
            L("Permission:Approvals")
        );
        approvalsPermission.AddChild(AuditPermissions.Approvals.Create, L("Permission:Approvals.Create"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Approve, L("Permission:Approvals.Approve"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Reject, L("Permission:Approvals.Reject"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Cancel, L("Permission:Approvals.Cancel"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Reassign, L("Permission:Approvals.Reassign"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Delete, L("Permission:Approvals.Delete"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.ViewAll, L("Permission:Approvals.ViewAll"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.ViewOwn, L("Permission:Approvals.ViewOwn"));

        // Reports Permissions
        var reportsPermission = auditGroup.AddPermission(
            AuditPermissions.Reports.Default,
            L("Permission:Reports")
        );
        reportsPermission.AddChild(AuditPermissions.Reports.View, L("Permission:Reports.View"));
        reportsPermission.AddChild(AuditPermissions.Reports.Generate, L("Permission:Reports.Generate"));
        reportsPermission.AddChild(AuditPermissions.Reports.Export, L("Permission:Reports.Export"));

        // Settings Permissions
        var settingsPermission = auditGroup.AddPermission(
            AuditPermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(AuditPermissions.Settings.Manage, L("Permission:Settings.Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditResource>(name);
    }
}
