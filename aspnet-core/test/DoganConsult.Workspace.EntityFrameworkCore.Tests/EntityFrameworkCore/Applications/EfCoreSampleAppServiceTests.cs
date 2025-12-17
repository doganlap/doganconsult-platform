using DoganConsult.Workspace.Samples;
using Xunit;

namespace DoganConsult.Workspace.EntityFrameworkCore.Applications;

[Collection(WorkspaceTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<WorkspaceEntityFrameworkCoreTestModule>
{

}
