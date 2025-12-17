using Xunit;

namespace DoganConsult.Identity.EntityFrameworkCore;

[CollectionDefinition(IdentityTestConsts.CollectionDefinitionName)]
public class IdentityEntityFrameworkCoreCollection : ICollectionFixture<IdentityEntityFrameworkCoreFixture>
{

}
