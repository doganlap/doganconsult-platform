using Volo.Abp.Modularity;

namespace DoganConsult.Workspace;

public abstract class WorkspaceApplicationTestBase<TStartupModule> : WorkspaceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
