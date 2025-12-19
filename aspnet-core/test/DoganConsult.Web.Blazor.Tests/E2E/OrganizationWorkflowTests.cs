using Microsoft.Playwright;
using Xunit;

namespace DoganConsult.Web.Blazor.Tests.E2E;

/// <summary>
/// E2E tests for organization workflow
/// </summary>
public class OrganizationWorkflowTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private const string BaseUrl = "https://localhost:44373";

    public OrganizationWorkflowTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task User_Can_Navigate_To_Organizations_Page()
    {
        var page = await _fixture.Browser.NewPageAsync();
        
        try
        {
            await page.GotoAsync(BaseUrl);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Navigate to organizations page
            var orgLink = page.Locator("a[href*='organizations'], a:has-text('Organizations')").First;
            if (await orgLink.IsVisibleAsync())
            {
                await orgLink.ClickAsync();
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Verify organizations page loaded
                var orgTable = page.Locator("table, .organizations-table").First;
                await orgTable.WaitForAsync(new LocatorWaitForOptions { Timeout = 5000 });
                
                var isVisible = await orgTable.IsVisibleAsync();
                Assert.True(isVisible, "Organizations table should be visible");
            }
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
