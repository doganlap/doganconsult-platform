using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Workspace.Workspaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class WorkspaceService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkspaceService> _logger;
    private readonly string _baseUrl;

    public WorkspaceService(HttpClient httpClient, ILogger<WorkspaceService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        var gatewayBaseUrl = configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        _baseUrl = $"{gatewayBaseUrl.TrimEnd('/')}/api/workspace/workspaces";
    }

    public async Task<PagedResultDto<WorkspaceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{_baseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                url += $"&Sorting={input.Sorting}";
            }
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<WorkspaceDto>>(url);
            return response ?? new PagedResultDto<WorkspaceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workspaces");
            return new PagedResultDto<WorkspaceDto>();
        }
    }

    public async Task<WorkspaceDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<WorkspaceDto>($"{_baseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching workspace {Id}", id);
            return null;
        }
    }

    public async Task<WorkspaceDto?> CreateAsync(CreateUpdateWorkspaceDto input)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WorkspaceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workspace");
            throw;
        }
    }

    public async Task<WorkspaceDto?> UpdateAsync(Guid id, CreateUpdateWorkspaceDto input)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<WorkspaceDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workspace {Id}", id);
            throw;
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{_baseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workspace {Id}", id);
            throw;
        }
    }
}
