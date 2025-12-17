using DoganConsult.Organization.Samples;
using Xunit;

namespace DoganConsult.Organization.EntityFrameworkCore.Applications;

[Collection(OrganizationTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<OrganizationEntityFrameworkCoreTestModule>
{

}
