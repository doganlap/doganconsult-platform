using DoganConsult.Workspace.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.Workspace.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(WorkspaceEntityFrameworkCoreModule),
    typeof(WorkspaceApplicationContractsModule)
    )]
public class WorkspaceDbMigratorModule : AbpModule
{
}
