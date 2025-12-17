using DoganConsult.Document.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Document.Permissions;

public class DocumentPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(DocumentPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(DocumentPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DocumentResource>(name);
    }
}
