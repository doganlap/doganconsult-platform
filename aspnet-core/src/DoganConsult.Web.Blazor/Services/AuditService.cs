using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Audit.AuditLogs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class AuditService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditService> _logger;
    private readonly string _baseUrl;

    public AuditService(HttpClient httpClient, ILogger<AuditService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        var gatewayBaseUrl = configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        _baseUrl = $"{gatewayBaseUrl.TrimEnd('/')}/api/audit/audit-logs";
    }

    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{_baseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                url += $"&Sorting={input.Sorting}";
            }

            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<AuditLogDto>>(url);
            return response ?? new PagedResultDto<AuditLogDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching audit logs");
            return new PagedResultDto<AuditLogDto>();
        }
    }

    public async Task<AuditLogDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AuditLogDto>($"{_baseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching audit log {Id}", id);
            return null;
        }
    }

    public async Task<AuditLogDto?> CreateAsync(CreateAuditLogDto input)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<AuditLogDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating audit log");
            throw;
        }
    }
}
