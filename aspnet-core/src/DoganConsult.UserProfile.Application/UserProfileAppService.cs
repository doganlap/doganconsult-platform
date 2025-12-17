using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.UserProfile.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.UserProfile;

/* Inherit your application services from this class.
 */
public abstract class UserProfileAppService : ApplicationService
{
    protected UserProfileAppService()
    {
        LocalizationResource = typeof(UserProfileResource);
    }
}
