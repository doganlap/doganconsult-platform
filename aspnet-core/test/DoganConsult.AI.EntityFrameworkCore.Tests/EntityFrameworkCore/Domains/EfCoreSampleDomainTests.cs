using DoganConsult.AI.Samples;
using Xunit;

namespace DoganConsult.AI.EntityFrameworkCore.Domains;

[Collection(AITestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AIEntityFrameworkCoreTestModule>
{

}
