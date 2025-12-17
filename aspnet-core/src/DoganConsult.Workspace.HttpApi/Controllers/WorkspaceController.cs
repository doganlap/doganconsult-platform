using DoganConsult.Workspace.Localization;
using DoganConsult.Workspace.Workspaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Workspace.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class WorkspaceController : AbpControllerBase
{
    protected WorkspaceController()
    {
        LocalizationResource = typeof(WorkspaceResource);
    }
}

[Route("api/workspace/workspaces")]
public class WorkspaceApiController : WorkspaceController
{
    private readonly IWorkspaceAppService _workspaceAppService;

    public WorkspaceApiController(IWorkspaceAppService workspaceAppService)
    {
        _workspaceAppService = workspaceAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<WorkspaceDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _workspaceAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<WorkspaceDto> GetAsync(Guid id)
    {
        return _workspaceAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<WorkspaceDto> CreateAsync([FromBody] CreateUpdateWorkspaceDto input)
    {
        return _workspaceAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<WorkspaceDto> UpdateAsync(Guid id, [FromBody] CreateUpdateWorkspaceDto input)
    {
        return _workspaceAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _workspaceAppService.DeleteAsync(id);
    }
}
