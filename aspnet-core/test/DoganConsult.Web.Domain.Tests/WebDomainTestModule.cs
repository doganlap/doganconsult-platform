using Volo.Abp.Modularity;

namespace DoganConsult.Web;

[DependsOn(
    typeof(WebDomainModule),
    typeof(WebTestBaseModule)
)]
public class WebDomainTestModule : AbpModule
{

}
