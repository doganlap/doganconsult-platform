using System;
using System.Threading.Tasks;
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
    Task<long> GetCountAsync();
}
