using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Audit.AuditLogs;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class AuditService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AuditService> _logger;
    private const string BaseUrl = "https://localhost:44375/api/audit/audit-logs";

    public AuditService(HttpClient httpClient, ILogger<AuditService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResultDto<AuditLogDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{BaseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
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
            return await _httpClient.GetFromJsonAsync<AuditLogDto>($"{BaseUrl}/{id}");
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
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, input);
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
