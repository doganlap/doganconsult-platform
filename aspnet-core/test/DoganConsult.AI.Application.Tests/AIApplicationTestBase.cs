using Volo.Abp.Modularity;

namespace DoganConsult.AI;

public abstract class AIApplicationTestBase<TStartupModule> : AITestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
