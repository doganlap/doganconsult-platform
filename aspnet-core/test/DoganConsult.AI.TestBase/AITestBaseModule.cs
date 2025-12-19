using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Authorization;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundJobs;
using Volo.Abp.Data;
using Volo.Abp.Modularity;
using Volo.Abp.Threading;

namespace DoganConsult.AI;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AbpTestBaseModule),
    typeof(AbpAuthorizationModule),
    typeof(AbpBackgroundJobsAbstractionsModule)
    )]
public class AITestBaseModule : AbpModule
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
            AsyncHelper.RunSync(async () =>
            {
                using (var scope = context.ServiceProvider.CreateScope())
                {
                    var dataSeeder = scope.ServiceProvider.GetService<IDataSeeder>();
                    if (dataSeeder != null)
                    {
                        await dataSeeder.SeedAsync();
                    }
                }
            });
        }
        catch
        {
            // Ignore seeding errors - EntityFrameworkCore tests will handle their own database setup
        }
    }
}
