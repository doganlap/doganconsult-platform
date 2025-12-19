using Microsoft.Playwright;
using Xunit;

namespace DoganConsult.Web.Blazor.Tests.E2E;

/// <summary>
/// E2E tests for user login flow
/// </summary>
public class LoginFlowTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;
    private const string BaseUrl = "https://localhost:44373";

    public LoginFlowTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task User_Can_Login_And_View_Dashboard()
    {
        var page = await _fixture.Browser.NewPageAsync();
        
        try
        {
            await page.GotoAsync(BaseUrl);
            
            // Wait for login form or dashboard
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // If login form exists, perform login
            var usernameInput = page.Locator("input[type='text'], input[name='username'], input[id='username']").First;
            if (await usernameInput.IsVisibleAsync())
            {
                await usernameInput.FillAsync("admin");
                
                var passwordInput = page.Locator("input[type='password'], input[name='password'], input[id='password']").First;
                await passwordInput.FillAsync("1q2w3E*");
                
                var submitButton = page.Locator("button[type='submit'], button:has-text('Login'), button:has-text('Sign in')").First;
                await submitButton.ClickAsync();
                
                // Wait for navigation after login
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            
            // Verify dashboard or main content is visible
            var dashboard = page.Locator(".dashboard, .container, main, [role='main']").First;
            await dashboard.WaitForAsync(new LocatorWaitForOptions { Timeout = 10000 });
            
            var isVisible = await dashboard.IsVisibleAsync();
            Assert.True(isVisible, "Dashboard should be visible after login");
        }
        finally
        {
            await page.CloseAsync();
        }
    }
}
