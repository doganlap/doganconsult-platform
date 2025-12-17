using Microsoft.Extensions.Localization;
using DoganConsult.Audit.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.Audit;

[Dependency(ReplaceServices = true)]
public class AuditBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AuditResource> _localizer;

    public AuditBrandingProvider(IStringLocalizer<AuditResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
