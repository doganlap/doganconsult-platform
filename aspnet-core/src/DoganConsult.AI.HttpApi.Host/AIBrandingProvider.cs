using Microsoft.Extensions.Localization;
using DoganConsult.AI.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.AI;

[Dependency(ReplaceServices = true)]
public class AIBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AIResource> _localizer;

    public AIBrandingProvider(IStringLocalizer<AIResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
