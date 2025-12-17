using Microsoft.Extensions.Localization;
using DoganConsult.UserProfile.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace DoganConsult.UserProfile;

[Dependency(ReplaceServices = true)]
public class UserProfileBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<UserProfileResource> _localizer;

    public UserProfileBrandingProvider(IStringLocalizer<UserProfileResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
