using Xunit;

namespace DoganConsult.Audit.EntityFrameworkCore;

[CollectionDefinition(AuditTestConsts.CollectionDefinitionName)]
public class AuditEntityFrameworkCoreCollection : ICollectionFixture<AuditEntityFrameworkCoreFixture>
{

}
