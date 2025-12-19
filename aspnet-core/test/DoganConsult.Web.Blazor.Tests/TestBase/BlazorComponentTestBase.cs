using Bunit;
using DoganConsult.Web.Blazor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Moq;
using System.Net.Http;
using System.Security.Claims;
using Volo.Abp.Authorization.Permissions;

namespace DoganConsult.Web.Blazor.Tests.TestBase;

/// <summary>
/// Base class for Blazor component tests with all required service mocks
/// </summary>
public abstract class BlazorComponentTestBase : TestContext
{
    protected Mock<IJSRuntime> MockJSRuntime { get; }
    protected Mock<IPermissionChecker> MockPermissionChecker { get; }
    protected Mock<IAuthorizationService> MockAuthorizationService { get; }
    protected Mock<AuthenticationStateProvider> MockAuthStateProvider { get; }
    protected Mock<IConfiguration> MockConfiguration { get; }
    protected HttpClient HttpClient { get; }
    protected Mock<ILoggerFactory> MockLoggerFactory { get; }

    protected BlazorComponentTestBase()
    {
        // Setup configuration mock
        MockConfiguration = new Mock<IConfiguration>();
        MockConfiguration.Setup(c => c["RemoteServices:Default:BaseUrl"]).Returns("http://localhost:5000");

        // Setup HttpClient (use real instance, not mock, for service constructors)
        HttpClient = new HttpClient();

        // Setup JS Runtime mock
        MockJSRuntime = new Mock<IJSRuntime>();
        MockJSRuntime.Setup(js => js.InvokeAsync<object?>(
            It.IsAny<string>(),
            It.IsAny<object?[]>()
        )).ReturnsAsync((object?)null);

        // Setup Permission Checker mock (default to allowing all permissions)
        MockPermissionChecker = new Mock<IPermissionChecker>();
        MockPermissionChecker.Setup(p => p.IsGrantedAsync(It.IsAny<string>()))
            .ReturnsAsync(true);

        // Setup Authorization Service mock (for UserProfiles component)
        MockAuthorizationService = new Mock<IAuthorizationService>();
        MockAuthorizationService.Setup(a => a.AuthorizeAsync(
            It.IsAny<System.Security.Claims.ClaimsPrincipal>(),
            It.IsAny<object>(),
            It.IsAny<string>()
        )).ReturnsAsync(AuthorizationResult.Success());

        // Setup Authentication State Provider mock (for Index component)
        MockAuthStateProvider = new Mock<AuthenticationStateProvider>();
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.Email, "test@example.com"),
            new Claim(ClaimTypes.Role, "User")
        };
        var identity = new ClaimsIdentity(claims, "test");
        var principal = new ClaimsPrincipal(identity);
        var authState = new AuthenticationState(principal);
        MockAuthStateProvider.Setup(a => a.GetAuthenticationStateAsync())
            .ReturnsAsync(authState);

        // Setup Logger Factory
        MockLoggerFactory = new Mock<ILoggerFactory>();
        var mockLogger = new Mock<ILogger>();
        MockLoggerFactory.Setup(f => f.CreateLogger(It.IsAny<string>()))
            .Returns(mockLogger.Object);

        // Register all mocks in DI container
        Services.AddSingleton(MockJSRuntime.Object);
        Services.AddSingleton(MockPermissionChecker.Object);
        Services.AddSingleton(MockAuthorizationService.Object);
        Services.AddSingleton<AuthenticationStateProvider>(MockAuthStateProvider.Object);
        Services.AddSingleton(MockConfiguration.Object);
        Services.AddSingleton(HttpClient);
        Services.AddSingleton(MockLoggerFactory.Object);

        // Register service factories that components need
        RegisterServices();
    }

    protected virtual void RegisterServices()
    {
        // Register OrganizationService
        Services.AddScoped<OrganizationService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<OrganizationService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new OrganizationService(httpClient, logger, config);
        });

        // Register WorkspaceService
        Services.AddScoped<WorkspaceService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<WorkspaceService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new WorkspaceService(httpClient, logger, config);
        });

        // Register DocumentService
        Services.AddScoped<DocumentService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<DocumentService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new DocumentService(httpClient, logger, config);
        });

        // Register UserProfileService
        Services.AddScoped<UserProfileService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<UserProfileService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new UserProfileService(httpClient, logger, config);
        });

        // Register AuditService
        Services.AddScoped<AuditService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<AuditService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new AuditService(httpClient, logger, config);
        });

        // Register AIService
        Services.AddScoped<AIService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<AIService>>();
            var config = sp.GetRequiredService<IConfiguration>();
            return new AIService(httpClient, logger, config);
        });

        // Register ApprovalService
        Services.AddScoped<ApprovalService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<ApprovalService>>();
            return new ApprovalService(httpClient, logger);
        });

        // Register DashboardService (doesn't take IConfiguration)
        Services.AddScoped<DashboardService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var logger = sp.GetRequiredService<ILogger<DashboardService>>();
            return new DashboardService(httpClient, logger);
        });

        // Register DemoService (for Index component)
        Services.AddScoped<DemoService>(sp =>
        {
            var httpClient = sp.GetRequiredService<HttpClient>();
            var navManager = sp.GetRequiredService<NavigationManager>();
            return new DemoService(httpClient, navManager);
        });

        // Register NavigationManager
        Services.AddSingleton<NavigationManager>(new TestNavigationManager());
    }

    /// <summary>
    /// Helper method to safely find an element, returns null if not found
    /// </summary>
    protected TElement? TryFindElement<TElement>(IRenderedFragment component, string selector) where TElement : class
    {
        try
        {
            return component.Find(selector) as TElement;
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Test implementation of NavigationManager
/// </summary>
public class TestNavigationManager : NavigationManager
{
    public TestNavigationManager()
    {
        Initialize("http://localhost:5000/", "http://localhost:5000/");
    }

    protected override void NavigateToCore(string uri, bool forceLoad)
    {
        // No-op for tests
    }
}
