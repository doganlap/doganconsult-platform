using Volo.Abp.Modularity;

namespace DoganConsult.Document;

/* Inherit from this class for your domain layer tests. */
public abstract class DocumentDomainTestBase<TStartupModule> : DocumentTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
