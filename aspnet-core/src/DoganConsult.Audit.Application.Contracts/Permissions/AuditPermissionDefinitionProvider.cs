using DoganConsult.Audit.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Audit.Permissions;

public class AuditPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AuditPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(AuditPermissions.MyPermission1, L("Permission:MyPermission1"));

        var approvalsPermission = myGroup.AddPermission(AuditPermissions.Approvals.Default, L("Permission:Approvals"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Create, L("Permission:Approvals.Create"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Approve, L("Permission:Approvals.Approve"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Reject, L("Permission:Approvals.Reject"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Cancel, L("Permission:Approvals.Cancel"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Reassign, L("Permission:Approvals.Reassign"));
        approvalsPermission.AddChild(AuditPermissions.Approvals.Delete, L("Permission:Approvals.Delete"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditResource>(name);
    }
}
