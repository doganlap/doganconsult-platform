using Volo.Abp.Modularity;

namespace DoganConsult.Workspace;

[DependsOn(
    typeof(WorkspaceDomainModule),
    typeof(WorkspaceTestBaseModule)
)]
public class WorkspaceDomainTestModule : AbpModule
{

}
