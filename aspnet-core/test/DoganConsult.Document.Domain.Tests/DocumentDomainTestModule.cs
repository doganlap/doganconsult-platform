using Volo.Abp.Modularity;

namespace DoganConsult.Document;

[DependsOn(
    typeof(DocumentDomainModule),
    typeof(DocumentTestBaseModule)
)]
public class DocumentDomainTestModule : AbpModule
{

}
