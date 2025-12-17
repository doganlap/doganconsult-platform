using System;
using System.Threading.Tasks;
using DoganConsult.Workspace.Branding;
using Microsoft.JSInterop;

namespace DoganConsult.Web.Blazor.Services;

public class DgThemeService
{
    private readonly IBrandingAppService _branding;
    private readonly IJSRuntime _js;

    public BrandingProfileDto Current { get; private set; } = new();

    public DgThemeService(IBrandingAppService branding, IJSRuntime js)
    {
        _branding = branding;
        _js = js;
    }

    public async Task InitializeAsync()
    {
        try
        {
            Current = await _branding.GetCurrentAsync();
            await ApplyThemeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing theme: {ex.Message}");
        }
    }

    public async Task ApplyThemeAsync()
    {
        try
        {
            await _js.InvokeVoidAsync("dgTheme.apply", new
            {
                primary = Current.PrimaryColor,
                accent = Current.AccentColor,
                appName = Current.AppDisplayName,
                rtl = Current.IsRtl,
                logoUrl = Current.LogoUrl,
                faviconUrl = Current.FaviconUrl
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying theme: {ex.Message}");
        }
    }

    public async Task UpdateBrandingAsync(BrandingProfileDto branding)
    {
        Current = await _branding.UpdateAsync(branding);
        await ApplyThemeAsync();
    }
}
