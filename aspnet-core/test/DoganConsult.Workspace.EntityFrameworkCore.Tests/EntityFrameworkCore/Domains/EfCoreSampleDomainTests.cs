using DoganConsult.Workspace.Samples;
using Xunit;

namespace DoganConsult.Workspace.EntityFrameworkCore.Domains;

[Collection(WorkspaceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<WorkspaceEntityFrameworkCoreTestModule>
{

}
