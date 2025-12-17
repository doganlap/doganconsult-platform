using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Organization.HttpClients;

public interface IIdentityServiceClient
{
    Task<bool> ValidateTokenAsync(string token);
    Task<string?> GetUserInfoAsync(string token);
}

public class IdentityServiceClient : IIdentityServiceClient, ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<IdentityServiceClient> _logger;

    public IdentityServiceClient(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<IdentityServiceClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;

        var identityBaseUrl = _configuration["Services:Identity:BaseUrl"] 
            ?? throw new InvalidOperationException("Services:Identity:BaseUrl is not configured");
        _httpClient.BaseAddress = new Uri(identityBaseUrl);
    }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/connect/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to validate token with Identity Service");
            return false;
        }
    }

    public async Task<string?> GetUserInfoAsync(string token)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/connect/userinfo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user info from Identity Service");
            return null;
        }
    }
}
