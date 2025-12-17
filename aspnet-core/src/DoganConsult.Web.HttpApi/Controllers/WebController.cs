using DoganConsult.Web.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Web.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WebController : AbpControllerBase
{
    protected WebController()
    {
        LocalizationResource = typeof(WebResource);
    }
}
