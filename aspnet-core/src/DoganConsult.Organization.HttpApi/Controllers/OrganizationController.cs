using DoganConsult.Organization.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Organization.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class OrganizationController : AbpControllerBase
{
    protected OrganizationController()
    {
        LocalizationResource = typeof(OrganizationResource);
    }
}
