using Volo.Abp.Account;
using Volo.Abp.Mapperly;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using DoganConsult.AI.Infrastructure;

namespace DoganConsult.AI;

[DependsOn(
    typeof(AIDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(AIApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AIInfrastructureModule)
    )]
public class AIApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMapperlyObjectMapper<AIApplicationModule>();
        
        // Register HttpClient for LLM service
        context.Services.AddHttpClient();
        context.Services.AddHttpContextAccessor();
    }
}
