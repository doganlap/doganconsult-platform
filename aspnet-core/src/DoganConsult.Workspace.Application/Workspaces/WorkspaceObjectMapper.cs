using System.Collections.Generic;
using DoganConsult.Workspace.Workspaces;
using Riok.Mapperly.Abstractions;

namespace DoganConsult.Workspace;

[Mapper]
public partial class WorkspaceObjectMapper
{
    [MapperIgnoreSource(nameof(Workspaces.Workspace.ExtraProperties))]
    [MapperIgnoreSource(nameof(Workspaces.Workspace.ConcurrencyStamp))]
    public partial WorkspaceDto ToWorkspaceDto(Workspace.Workspaces.Workspace workspace);
    public partial List<WorkspaceDto> ToWorkspaceDtos(List<Workspace.Workspaces.Workspace> workspaces);
}
