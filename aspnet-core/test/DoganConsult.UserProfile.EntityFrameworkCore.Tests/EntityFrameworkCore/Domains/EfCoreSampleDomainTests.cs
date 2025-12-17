using DoganConsult.UserProfile.Samples;
using Xunit;

namespace DoganConsult.UserProfile.EntityFrameworkCore.Domains;

[Collection(UserProfileTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<UserProfileEntityFrameworkCoreTestModule>
{

}
