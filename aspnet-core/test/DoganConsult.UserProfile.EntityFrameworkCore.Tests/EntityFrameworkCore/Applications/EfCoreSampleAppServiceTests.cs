using DoganConsult.UserProfile.Samples;
using Xunit;

namespace DoganConsult.UserProfile.EntityFrameworkCore.Applications;

[Collection(UserProfileTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<UserProfileEntityFrameworkCoreTestModule>
{

}
