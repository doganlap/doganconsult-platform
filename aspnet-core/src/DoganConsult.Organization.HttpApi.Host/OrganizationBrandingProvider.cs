using Microsoft.Extensions.Localization;
using DoganConsult.Organization.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.Organization;

[Dependency(ReplaceServices = true)]
public class OrganizationBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<OrganizationResource> _localizer;

    public OrganizationBrandingProvider(IStringLocalizer<OrganizationResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
