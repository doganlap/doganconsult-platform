using DoganConsult.Identity.Samples;
using Xunit;

namespace DoganConsult.Identity.EntityFrameworkCore.Domains;

[Collection(IdentityTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<IdentityEntityFrameworkCoreTestModule>
{

}
