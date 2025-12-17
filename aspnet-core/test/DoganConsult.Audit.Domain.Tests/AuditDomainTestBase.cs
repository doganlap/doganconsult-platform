using Volo.Abp.Modularity;

namespace DoganConsult.Audit;

/* Inherit from this class for your domain layer tests. */
public abstract class AuditDomainTestBase<TStartupModule> : AuditTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
