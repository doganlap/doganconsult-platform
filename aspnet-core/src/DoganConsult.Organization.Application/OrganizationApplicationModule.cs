using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.DependencyInjection;

namespace DoganConsult.Organization;

[DependsOn(
    typeof(OrganizationDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(OrganizationApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class OrganizationApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<OrganizationApplicationModule>();
        
        // Manually register the mapper to ensure it's available for DI
        context.Services.AddSingleton<OrganizationApplicationMappers>();
        
        // Add HTTP client factory for inter-service communication
        context.Services.AddHttpClient();
        context.Services.AddHttpContextAccessor();
    }
}
