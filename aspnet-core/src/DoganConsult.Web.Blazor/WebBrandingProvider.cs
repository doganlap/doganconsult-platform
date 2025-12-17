using Microsoft.Extensions.Localization;
using DoganConsult.Web.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.Web.Blazor;

[Dependency(ReplaceServices = true)]
public class WebBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<WebResource> _localizer;

    public WebBrandingProvider(IStringLocalizer<WebResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
