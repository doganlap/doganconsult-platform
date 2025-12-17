using Microsoft.Extensions.Localization;
using DoganConsult.Document.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.Document;

[Dependency(ReplaceServices = true)]
public class DocumentBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<DocumentResource> _localizer;

    public DocumentBrandingProvider(IStringLocalizer<DocumentResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
