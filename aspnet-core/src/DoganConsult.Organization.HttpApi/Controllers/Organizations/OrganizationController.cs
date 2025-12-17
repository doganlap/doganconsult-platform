using DoganConsult.Organization.Organizations;
using DoganConsult.Organization.Localization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Organization.Controllers.Organizations;

[Route("api/organization/organizations")]
public class OrganizationApiController : AbpControllerBase
{
    private readonly IOrganizationAppService _organizationAppService;

    public OrganizationApiController(IOrganizationAppService organizationAppService)
    {
        _organizationAppService = organizationAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<OrganizationDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _organizationAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<OrganizationDto> GetAsync(Guid id)
    {
        return _organizationAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<OrganizationDto> CreateAsync([FromBody] CreateUpdateOrganizationDto input)
    {
        return _organizationAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<OrganizationDto> UpdateAsync(Guid id, [FromBody] CreateUpdateOrganizationDto input)
    {
        return _organizationAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _organizationAppService.DeleteAsync(id);
    }
}
