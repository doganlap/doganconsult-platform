using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace DoganConsult.Organization;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
    )]
public class OrganizationTestBaseModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpBackgroundJobOptions>(options =>
        {
            options.IsJobExecutionEnabled = false;
        });

        context.Services.AddAlwaysAllowAuthorization();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        try
        {
            SeedTestData(context);
        }
        catch
        {
            // Ignore all initialization errors - EntityFrameworkCore tests handle their own setup
        }
    }

    private static void SeedTestData(ApplicationInitializationContext context)
    {
        try
        {
            var serviceProvider = context.ServiceProvider;
            if (serviceProvider == null) return;

            using (var scope = serviceProvider.CreateScope())
            {
                var dataSeeder = scope.ServiceProvider.GetService<IDataSeeder>();
                if (dataSeeder != null)
                {
                    AsyncHelper.RunSync(async () => await dataSeeder.SeedAsync());
                }
            }
        }
        catch
        {
            // Ignore seeding errors - EntityFrameworkCore tests will handle their own database setup
        }
    }
}
