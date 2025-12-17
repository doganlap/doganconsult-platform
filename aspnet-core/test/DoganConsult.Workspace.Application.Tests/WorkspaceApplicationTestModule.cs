using Volo.Abp.Modularity;

namespace DoganConsult.Workspace;

[DependsOn(
    typeof(WorkspaceApplicationModule),
    typeof(WorkspaceDomainTestModule)
)]
public class WorkspaceApplicationTestModule : AbpModule
{

}
