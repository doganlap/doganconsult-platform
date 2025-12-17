using Xunit;

namespace DoganConsult.AI.EntityFrameworkCore;

[CollectionDefinition(AITestConsts.CollectionDefinitionName)]
public class AIEntityFrameworkCoreCollection : ICollectionFixture<AIEntityFrameworkCoreFixture>
{

}
