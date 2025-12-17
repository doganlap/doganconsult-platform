using Volo.Abp.Modularity;

namespace DoganConsult.Identity;

[DependsOn(
    typeof(IdentityDomainModule),
    typeof(IdentityTestBaseModule)
)]
public class IdentityDomainTestModule : AbpModule
{

}
