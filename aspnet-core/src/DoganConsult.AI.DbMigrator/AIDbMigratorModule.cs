using DoganConsult.AI.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.AI.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AIEntityFrameworkCoreModule),
    typeof(AIApplicationContractsModule)
    )]
public class AIDbMigratorModule : AbpModule
{
}
