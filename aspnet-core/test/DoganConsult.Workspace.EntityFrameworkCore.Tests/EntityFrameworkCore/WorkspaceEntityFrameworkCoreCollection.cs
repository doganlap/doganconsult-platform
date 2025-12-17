using Xunit;

namespace DoganConsult.Workspace.EntityFrameworkCore;

[CollectionDefinition(WorkspaceTestConsts.CollectionDefinitionName)]
public class WorkspaceEntityFrameworkCoreCollection : ICollectionFixture<WorkspaceEntityFrameworkCoreFixture>
{

}
