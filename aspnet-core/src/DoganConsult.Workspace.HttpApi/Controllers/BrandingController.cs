using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using DoganConsult.Workspace.Branding;

namespace DoganConsult.Workspace.Controllers;

[Route("api/dg/branding")]
public class BrandingController : AbpControllerBase
{
    private readonly IBrandingAppService _service;

    public BrandingController(IBrandingAppService service) => _service = service;

    [HttpGet("current")]
    public Task<BrandingProfileDto> GetCurrentAsync() => _service.GetCurrentAsync();

    [HttpPut("update")]
    public Task<BrandingProfileDto> UpdateAsync(BrandingProfileDto input) => _service.UpdateAsync(input);
}
