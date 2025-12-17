using Microsoft.Extensions.Localization;
using DoganConsult.Workspace.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.Workspace;

[Dependency(ReplaceServices = true)]
public class WorkspaceBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<WorkspaceResource> _localizer;

    public WorkspaceBrandingProvider(IStringLocalizer<WorkspaceResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
