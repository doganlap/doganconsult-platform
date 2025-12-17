using Volo.Abp.Modularity;

namespace DoganConsult.Workspace;

/* Inherit from this class for your domain layer tests. */
public abstract class WorkspaceDomainTestBase<TStartupModule> : WorkspaceTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
