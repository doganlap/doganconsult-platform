using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace DoganConsult.Web.Blazor.Middleware;

public class AutoLoginMiddleware : ITransientDependency
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AutoLoginMiddleware> _logger;

    public AutoLoginMiddleware(
        RequestDelegate next,
        IConfiguration configuration,
        ILogger<AutoLoginMiddleware> logger)
    {
        _next = next;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IIdentityUserRepository userRepository)
    {
        // Only auto-login in development mode and if not already authenticated
        var autoLoginEnabled = _configuration.GetValue<bool>("App:AutoLoginEnabled");
        
        if (autoLoginEnabled && context.User?.Identity?.IsAuthenticated != true)
        {
            var username = _configuration["App:AutoLoginUsername"];
            
            if (!string.IsNullOrEmpty(username))
            {
                try
                {
                    // Find the user
                    var user = await userRepository.FindByNormalizedUserNameAsync(username.ToUpperInvariant());
                    
                    if (user != null)
                    {
                        // Create claims list
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                            new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                            new Claim(AbpClaimTypes.UserId, user.Id.ToString()),
                            new Claim(AbpClaimTypes.UserName, user.UserName ?? string.Empty),
                            new Claim(AbpClaimTypes.Email, user.Email ?? string.Empty),
                            new Claim(AbpClaimTypes.EmailVerified, user.EmailConfirmed.ToString().ToLowerInvariant())
                        };

                        if (user.TenantId.HasValue)
                        {
                            claims.Add(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()!));
                        }

                        var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
                        var principal = new ClaimsPrincipal(identity);

                        // Sign in the user
                        await context.SignInAsync(
                            IdentityConstants.ApplicationScheme,
                            principal,
                            new AuthenticationProperties
                            {
                                IsPersistent = true,
                                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
                            });

                        _logger.LogInformation("Auto-login successful for user: {Username}", username);
                    }
                    else
                    {
                        _logger.LogWarning("Auto-login failed: User '{Username}' not found", username);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during auto-login");
                }
            }
        }

        await _next(context);
    }
}
