using DoganConsult.UserProfile.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace DoganConsult.UserProfile.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(UserProfileEntityFrameworkCoreModule),
    typeof(UserProfileApplicationContractsModule)
    )]
public class UserProfileDbMigratorModule : AbpModule
{
}
