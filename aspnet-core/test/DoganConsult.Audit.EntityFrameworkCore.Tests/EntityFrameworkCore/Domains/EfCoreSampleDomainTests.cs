using DoganConsult.Audit.Samples;
using Xunit;

namespace DoganConsult.Audit.EntityFrameworkCore.Domains;

[Collection(AuditTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<AuditEntityFrameworkCoreTestModule>
{

}
