using DoganConsult.Web.Samples;
using Xunit;

namespace DoganConsult.Web.EntityFrameworkCore.Applications;

[Collection(WebTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<WebEntityFrameworkCoreTestModule>
{

}
