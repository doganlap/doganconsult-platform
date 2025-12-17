using Volo.Abp.Modularity;

namespace DoganConsult.UserProfile;

[DependsOn(
    typeof(UserProfileDomainModule),
    typeof(UserProfileTestBaseModule)
)]
public class UserProfileDomainTestModule : AbpModule
{

}
