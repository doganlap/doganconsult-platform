using Volo.Abp.Modularity;

namespace DoganConsult.Organization;

public abstract class OrganizationApplicationTestBase<TStartupModule> : OrganizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
