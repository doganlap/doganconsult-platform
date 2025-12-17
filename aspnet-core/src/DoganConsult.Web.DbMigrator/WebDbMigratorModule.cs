using DoganConsult.Web.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Web.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WebEntityFrameworkCoreModule),
    typeof(WebApplicationContractsModule)
    )]
public class WebDbMigratorModule : AbpModule
{
}
