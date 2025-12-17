using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DoganConsult.Workspace.Branding;

public interface IBrandingAppService : IApplicationService
{
    Task<BrandingProfileDto> GetCurrentAsync();
    Task<BrandingProfileDto> UpdateAsync(BrandingProfileDto input);
}
