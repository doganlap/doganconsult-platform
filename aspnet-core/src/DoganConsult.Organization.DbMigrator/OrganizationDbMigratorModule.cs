using DoganConsult.Organization.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Organization.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(OrganizationEntityFrameworkCoreModule),
    typeof(OrganizationApplicationContractsModule)
    )]
public class OrganizationDbMigratorModule : AbpModule
{
}
