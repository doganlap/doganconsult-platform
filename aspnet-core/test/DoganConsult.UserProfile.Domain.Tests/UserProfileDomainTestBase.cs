using Volo.Abp.Modularity;

namespace DoganConsult.UserProfile;

/* Inherit from this class for your domain layer tests. */
public abstract class UserProfileDomainTestBase<TStartupModule> : UserProfileTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
