using Volo.Abp.Modularity;

namespace DoganConsult.Document;

public abstract class DocumentApplicationTestBase<TStartupModule> : DocumentTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
