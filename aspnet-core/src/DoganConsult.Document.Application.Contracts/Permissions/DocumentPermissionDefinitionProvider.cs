using DoganConsult.Document.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace DoganConsult.Document.Permissions;

public class DocumentPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var documentGroup = context.AddGroup(DocumentPermissions.GroupName, L("Permission:DocumentManagement"));

        // Document Permissions
        var documentsPermission = documentGroup.AddPermission(
            DocumentPermissions.Documents.Default,
            L("Permission:Documents")
        );
        documentsPermission.AddChild(DocumentPermissions.Documents.Create, L("Permission:Documents.Create"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Edit, L("Permission:Documents.Edit"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Delete, L("Permission:Documents.Delete"));
        documentsPermission.AddChild(DocumentPermissions.Documents.ViewAll, L("Permission:Documents.ViewAll"));
        documentsPermission.AddChild(DocumentPermissions.Documents.ViewOwn, L("Permission:Documents.ViewOwn"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Download, L("Permission:Documents.Download"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Upload, L("Permission:Documents.Upload"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Share, L("Permission:Documents.Share"));
        documentsPermission.AddChild(DocumentPermissions.Documents.Archive, L("Permission:Documents.Archive"));

        // Folder Permissions
        var foldersPermission = documentGroup.AddPermission(
            DocumentPermissions.Folders.Default,
            L("Permission:Folders")
        );
        foldersPermission.AddChild(DocumentPermissions.Folders.Create, L("Permission:Folders.Create"));
        foldersPermission.AddChild(DocumentPermissions.Folders.Manage, L("Permission:Folders.Manage"));
        foldersPermission.AddChild(DocumentPermissions.Folders.Delete, L("Permission:Folders.Delete"));

        // Settings Permissions
        var settingsPermission = documentGroup.AddPermission(
            DocumentPermissions.Settings.Default,
            L("Permission:Settings")
        );
        settingsPermission.AddChild(DocumentPermissions.Settings.Manage, L("Permission:Settings.Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<DocumentResource>(name);
    }
}
