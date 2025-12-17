using Volo.Abp.Modularity;

namespace DoganConsult.AI;

[DependsOn(
    typeof(AIApplicationModule),
    typeof(AIDomainTestModule)
)]
public class AIApplicationTestModule : AbpModule
{

}
