using Volo.Abp.Modularity;

namespace DoganConsult.UserProfile;

[DependsOn(
    typeof(UserProfileApplicationModule),
    typeof(UserProfileDomainTestModule)
)]
public class UserProfileApplicationTestModule : AbpModule
{

}
