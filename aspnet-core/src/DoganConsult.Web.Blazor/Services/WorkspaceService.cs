using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Workspace.Workspaces;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class WorkspaceService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<WorkspaceService> _logger;
    private const string BaseUrl = "https://localhost:44371/api/workspace/workspaces";

    public WorkspaceService(HttpClient httpClient, ILogger<WorkspaceService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResultDto<WorkspaceDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{BaseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
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
            return await _httpClient.GetFromJsonAsync<WorkspaceDto>($"{BaseUrl}/{id}");
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
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, input);
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
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", input);
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
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workspace {Id}", id);
            throw;
        }
    }
}
