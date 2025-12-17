using DoganConsult.AI.Samples;
using Xunit;

namespace DoganConsult.AI.EntityFrameworkCore.Applications;

[Collection(AITestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AIEntityFrameworkCoreTestModule>
{

}
