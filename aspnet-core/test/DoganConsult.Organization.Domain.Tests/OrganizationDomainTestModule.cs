using Volo.Abp.Modularity;

namespace DoganConsult.Organization;

[DependsOn(
    typeof(OrganizationDomainModule),
    typeof(OrganizationTestBaseModule)
)]
public class OrganizationDomainTestModule : AbpModule
{

}
