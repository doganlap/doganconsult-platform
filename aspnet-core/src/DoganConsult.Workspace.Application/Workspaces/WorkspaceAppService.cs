using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Workspace.Workspaces;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Workspace.Workspaces;

[Authorize]
public class WorkspaceAppService : ApplicationService, IWorkspaceAppService
{
    private readonly IRepository<Workspace, Guid> _workspaceRepository;

    public WorkspaceAppService(IRepository<Workspace, Guid> workspaceRepository)
    {
        _workspaceRepository = workspaceRepository;
    }

    public async Task<WorkspaceDto> CreateAsync(CreateUpdateWorkspaceDto input)
    {
        var workspace = new Workspace(
            GuidGenerator.Create(),
            input.Code,
            input.Name,
            input.OrganizationId,
            CurrentTenant.Id
        )
        {
            Description = input.Description,
            Status = input.Status ?? "active",
            Settings = input.Settings,
            Members = input.Members,
            CreatedDate = DateTime.UtcNow,
            WorkspaceOwner = CurrentUser.Id?.ToString(),
            Permissions = input.Permissions
        };

        await _workspaceRepository.InsertAsync(workspace);
        return ObjectMapper.Map<Workspace, WorkspaceDto>(workspace);
    }

    public async Task<WorkspaceDto> GetAsync(Guid id)
    {
        var workspace = await _workspaceRepository.GetAsync(id);
        return ObjectMapper.Map<Workspace, WorkspaceDto>(workspace);
    }

    public async Task<PagedResultDto<WorkspaceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _workspaceRepository.GetQueryableAsync();
        var workspaces = queryable
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        var totalCount = await _workspaceRepository.GetCountAsync();
        return new PagedResultDto<WorkspaceDto>(
            totalCount,
            ObjectMapper.Map<List<Workspace>, List<WorkspaceDto>>(workspaces)
        );
    }

    public async Task<WorkspaceDto> UpdateAsync(Guid id, CreateUpdateWorkspaceDto input)
    {
        var workspace = await _workspaceRepository.GetAsync(id);
        workspace.Code = input.Code;
        workspace.Name = input.Name;
        workspace.OrganizationId = input.OrganizationId;
        workspace.Description = input.Description;
        workspace.Status = input.Status ?? workspace.Status;
        workspace.Settings = input.Settings ?? workspace.Settings;
        workspace.Members = input.Members ?? workspace.Members;
        workspace.Permissions = input.Permissions ?? workspace.Permissions;
        await _workspaceRepository.UpdateAsync(workspace);
        return ObjectMapper.Map<Workspace, WorkspaceDto>(workspace);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _workspaceRepository.DeleteAsync(id);
    }
}
