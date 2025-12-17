using DoganConsult.Organization.Samples;
using Xunit;

namespace DoganConsult.Organization.EntityFrameworkCore.Domains;

[Collection(OrganizationTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<OrganizationEntityFrameworkCoreTestModule>
{

}
