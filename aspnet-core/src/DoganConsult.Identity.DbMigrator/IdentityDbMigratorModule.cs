using DoganConsult.Identity.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Identity.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(IdentityEntityFrameworkCoreModule),
    typeof(IdentityApplicationContractsModule)
    )]
public class IdentityDbMigratorModule : AbpModule
{
}
