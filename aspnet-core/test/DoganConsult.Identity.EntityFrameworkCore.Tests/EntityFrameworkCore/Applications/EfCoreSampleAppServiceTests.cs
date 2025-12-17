using DoganConsult.Identity.Samples;
using Xunit;

namespace DoganConsult.Identity.EntityFrameworkCore.Applications;

[Collection(IdentityTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<IdentityEntityFrameworkCoreTestModule>
{

}
