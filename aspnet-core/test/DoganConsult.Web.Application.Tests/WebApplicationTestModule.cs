using Volo.Abp.Modularity;

namespace DoganConsult.Web;

[DependsOn(
    typeof(WebApplicationModule),
    typeof(WebDomainTestModule)
)]
public class WebApplicationTestModule : AbpModule
{

}
