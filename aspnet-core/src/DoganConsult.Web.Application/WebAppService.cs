using System;
using System.Collections.Generic;
using System.Text;
using DoganConsult.Web.Localization;
using Volo.Abp.Application.Services;

namespace DoganConsult.Web;

/* Inherit your application services from this class.
 */
public abstract class WebAppService : ApplicationService
{
    protected WebAppService()
    {
        LocalizationResource = typeof(WebResource);
    }
}
