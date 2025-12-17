using Volo.Abp.Modularity;

namespace DoganConsult.Web;

/* Inherit from this class for your domain layer tests. */
public abstract class WebDomainTestBase<TStartupModule> : WebTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
