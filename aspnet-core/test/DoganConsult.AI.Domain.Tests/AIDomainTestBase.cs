using Volo.Abp.Modularity;

namespace DoganConsult.AI;

/* Inherit from this class for your domain layer tests. */
public abstract class AIDomainTestBase<TStartupModule> : AITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
