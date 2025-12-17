using System;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.Workspace.Workspaces;

public interface IWorkspaceAppService : ICrudAppService<
    WorkspaceDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateWorkspaceDto,
    CreateUpdateWorkspaceDto>
{
}
