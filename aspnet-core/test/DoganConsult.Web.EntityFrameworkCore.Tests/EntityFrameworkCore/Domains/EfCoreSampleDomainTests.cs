using DoganConsult.Web.Samples;
using Xunit;

namespace DoganConsult.Web.EntityFrameworkCore.Domains;

[Collection(WebTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<WebEntityFrameworkCoreTestModule>
{

}
