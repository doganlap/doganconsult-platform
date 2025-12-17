using Volo.Abp.Modularity;

namespace DoganConsult.Document;

[DependsOn(
    typeof(DocumentApplicationModule),
    typeof(DocumentDomainTestModule)
)]
public class DocumentApplicationTestModule : AbpModule
{

}
