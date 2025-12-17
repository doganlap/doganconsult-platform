using Xunit;

namespace DoganConsult.UserProfile.EntityFrameworkCore;

[CollectionDefinition(UserProfileTestConsts.CollectionDefinitionName)]
public class UserProfileEntityFrameworkCoreCollection : ICollectionFixture<UserProfileEntityFrameworkCoreFixture>
{

}
