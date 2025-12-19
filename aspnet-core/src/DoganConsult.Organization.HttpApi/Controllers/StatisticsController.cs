using DoganConsult.Organization.Organizations;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Organization.Controllers;

[Route("api/organization/statistics")]
public class StatisticsController : AbpControllerBase
{
    private readonly IOrganizationAppService _organizationAppService;

    public StatisticsController(IOrganizationAppService organizationAppService)
    {
        _organizationAppService = organizationAppService;
    }

    /// <summary>
    /// Get organization statistics including trends
    /// </summary>
    [HttpGet]
    public Task<OrganizationStatisticsDto> GetStatisticsAsync()
    {
        return _organizationAppService.GetStatisticsAsync();
    }

    /// <summary>
    /// Get organization trends data
    /// </summary>
    [HttpGet("trends")]
    public async Task<object> GetTrendsAsync()
    {
        var statistics = await _organizationAppService.GetStatisticsAsync();
        return new { Trends = statistics.Trends };
    }
}
