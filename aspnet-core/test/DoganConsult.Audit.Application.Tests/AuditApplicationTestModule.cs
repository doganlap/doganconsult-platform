using Volo.Abp.Modularity;

namespace DoganConsult.Audit;

[DependsOn(
    typeof(AuditApplicationModule),
    typeof(AuditDomainTestModule)
)]
public class AuditApplicationTestModule : AbpModule
{

}
