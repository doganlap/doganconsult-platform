using DoganConsult.Document.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Document.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(DocumentEntityFrameworkCoreModule),
    typeof(DocumentApplicationContractsModule)
    )]
public class DocumentDbMigratorModule : AbpModule
{
}
