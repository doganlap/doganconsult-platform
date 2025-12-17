using Volo.Abp.Modularity;

namespace DoganConsult.Web;

public abstract class WebApplicationTestBase<TStartupModule> : WebTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
