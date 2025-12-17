using DoganConsult.Audit.Samples;
using Xunit;

namespace DoganConsult.Audit.EntityFrameworkCore.Applications;

[Collection(AuditTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<AuditEntityFrameworkCoreTestModule>
{

}
