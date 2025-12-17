using DoganConsult.Web.Localization;
using Volo.Abp.AspNetCore.Components;

namespace DoganConsult.Web.Blazor;

public abstract class WebComponentBase : AbpComponentBase
{
    protected WebComponentBase()
    {
        LocalizationResource = typeof(WebResource);
    }
}
