using Volo.Abp.Modularity;

namespace DoganConsult.Audit;

public abstract class AuditApplicationTestBase<TStartupModule> : AuditTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
