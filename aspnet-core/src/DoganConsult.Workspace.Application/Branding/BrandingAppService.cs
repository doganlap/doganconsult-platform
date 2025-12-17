using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace DoganConsult.Workspace.Branding;

public class BrandingAppService : ApplicationService, IBrandingAppService
{
    private readonly IRepository<BrandingProfile, Guid> _repo;
    private readonly IDistributedCache<BrandingProfileDto> _cache;
    private readonly ICurrentTenant _currentTenant;

    public BrandingAppService(
        IRepository<BrandingProfile, Guid> repo,
        IDistributedCache<BrandingProfileDto> cache,
        ICurrentTenant currentTenant)
    {
        _repo = repo;
        _cache = cache;
        _currentTenant = currentTenant;
    }

    public async Task<BrandingProfileDto> GetCurrentAsync()
    {
        var tenantId = _currentTenant.Id;
        var cacheKey = $"dg:branding:{tenantId?.ToString() ?? "host"}";

        var cached = await _cache.GetAsync(cacheKey);
        if (cached != null) return cached;

        var profile = await _repo.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        var dto = profile == null
            ? new BrandingProfileDto()
            : ObjectMapper.Map<BrandingProfile, BrandingProfileDto>(profile);

        await _cache.SetAsync(cacheKey, dto);

        return dto;
    }

    public async Task<BrandingProfileDto> UpdateAsync(BrandingProfileDto input)
    {
        var tenantId = _currentTenant.Id;
        var profile = await _repo.FirstOrDefaultAsync(x => x.TenantId == tenantId);

        if (profile == null)
        {
            profile = new BrandingProfile(Guid.NewGuid(), tenantId);
            await _repo.InsertAsync(profile);
        }

        profile.AppDisplayName = input.AppDisplayName;
        profile.ProductName = input.ProductName;
        profile.LogoUrl = input.LogoUrl;
        profile.FaviconUrl = input.FaviconUrl;
        profile.PrimaryColor = input.PrimaryColor;
        profile.AccentColor = input.AccentColor;
        profile.DefaultLanguage = input.DefaultLanguage;
        profile.IsRtl = input.IsRtl;
        profile.HomeRoute = input.HomeRoute;

        await _repo.UpdateAsync(profile);

        var cacheKey = $"dg:branding:{tenantId?.ToString() ?? "host"}";
        await _cache.RemoveAsync(cacheKey);

        return await GetCurrentAsync();
    }
}
