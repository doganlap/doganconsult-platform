using Volo.Abp.Modularity;

namespace DoganConsult.Identity;

[DependsOn(
    typeof(IdentityApplicationModule),
    typeof(IdentityDomainTestModule)
)]
public class IdentityApplicationTestModule : AbpModule
{

}
