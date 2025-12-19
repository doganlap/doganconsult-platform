# Playwright E2E Tests Setup

## Installation

```bash
cd d:\test\aspnet-core\test\DoganConsult.Web.Blazor.Tests\E2E
dotnet new classlib -n DoganConsult.Web.Blazor.E2ETests
cd DoganConsult.Web.Blazor.E2ETests
dotnet add package Microsoft.Playwright
dotnet add package xunit
dotnet add package xunit.runner.visualstudio
```

## Install Playwright Browsers

```bash
pwsh bin/Debug/net10.0/playwright.ps1 install
```

## Example Test Structure

```csharp
using Microsoft.Playwright;
using Xunit;

public class LoginFlowTests : IClassFixture<PlaywrightFixture>
{
    private readonly PlaywrightFixture _fixture;

    public LoginFlowTests(PlaywrightFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task User_Can_Login_And_View_Dashboard()
    {
        var page = await _fixture.Browser.NewPageAsync();
        await page.GotoAsync("https://localhost:44373");
        
        // Login steps
        await page.FillAsync("#username", "admin");
        await page.FillAsync("#password", "1q2w3E*");
        await page.ClickAsync("button[type='submit']");
        
        // Verify dashboard
        await page.WaitForSelectorAsync(".dashboard");
        var title = await page.TextContentAsync("h1");
        Assert.Contains("Dashboard", title);
        
        await page.CloseAsync();
    }
}
```
