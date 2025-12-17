using Volo.Abp.Modularity;

namespace DoganConsult.AI;

[DependsOn(
    typeof(AIDomainModule),
    typeof(AITestBaseModule)
)]
public class AIDomainTestModule : AbpModule
{

}
