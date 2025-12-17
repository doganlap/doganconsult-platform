using Xunit;

namespace DoganConsult.Organization.EntityFrameworkCore;

[CollectionDefinition(OrganizationTestConsts.CollectionDefinitionName)]
public class OrganizationEntityFrameworkCoreCollection : ICollectionFixture<OrganizationEntityFrameworkCoreFixture>
{

}
