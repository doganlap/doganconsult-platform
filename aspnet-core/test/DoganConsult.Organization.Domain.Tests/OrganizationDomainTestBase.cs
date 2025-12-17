using Volo.Abp.Modularity;

namespace DoganConsult.Organization;

/* Inherit from this class for your domain layer tests. */
public abstract class OrganizationDomainTestBase<TStartupModule> : OrganizationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
