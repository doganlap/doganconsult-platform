using Volo.Abp.Modularity;

namespace DoganConsult.Organization;

[DependsOn(
    typeof(OrganizationApplicationModule),
    typeof(OrganizationDomainTestModule)
)]
public class OrganizationApplicationTestModule : AbpModule
{

}
