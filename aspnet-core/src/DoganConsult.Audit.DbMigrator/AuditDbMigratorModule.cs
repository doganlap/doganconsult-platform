using DoganConsult.Audit.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Audit.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AuditEntityFrameworkCoreModule),
    typeof(AuditApplicationContractsModule)
    )]
public class AuditDbMigratorModule : AbpModule
{
}
