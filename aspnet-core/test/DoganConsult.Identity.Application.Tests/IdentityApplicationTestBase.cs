using Volo.Abp.Modularity;

namespace DoganConsult.Identity;

public abstract class IdentityApplicationTestBase<TStartupModule> : IdentityTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
