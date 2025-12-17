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
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AuditResource>(name);
    }
}
