using Volo.Abp.Modularity;

namespace DoganConsult.Audit;

[DependsOn(
    typeof(AuditDomainModule),
    typeof(AuditTestBaseModule)
)]
public class AuditDomainTestModule : AbpModule
{

}
