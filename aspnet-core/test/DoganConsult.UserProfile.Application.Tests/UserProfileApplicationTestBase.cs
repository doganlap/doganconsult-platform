using Volo.Abp.Modularity;

namespace DoganConsult.UserProfile;

public abstract class UserProfileApplicationTestBase<TStartupModule> : UserProfileTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
