using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Organization.HttpClients;

public interface IServiceHttpClientHelper
{
    HttpClient CreateClient(string serviceName);
    Task<HttpClient> CreateAuthenticatedClientAsync(string serviceName);
}

public class ServiceHttpClientHelper : IServiceHttpClientHelper, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceHttpClientHelper(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _httpContextAccessor = httpContextAccessor;
    }

    public HttpClient CreateClient(string serviceName)
    {
        var client = _httpClientFactory.CreateClient();
        var baseUrl = _configuration[$"Services:{serviceName}:BaseUrl"];
        if (!string.IsNullOrEmpty(baseUrl))
        {
            client.BaseAddress = new Uri(baseUrl);
        }
        return client;
    }

    public async Task<HttpClient> CreateAuthenticatedClientAsync(string serviceName)
    {
        var client = CreateClient(serviceName);
        
        // Forward JWT token from current request
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext != null)
        {
            var token = httpContext.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(token) && token.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Substring(7));
            }
        }

        return await Task.FromResult(client);
    }
}
