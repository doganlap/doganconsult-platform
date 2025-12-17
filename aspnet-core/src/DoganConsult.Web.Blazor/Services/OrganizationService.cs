using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using DoganConsult.Web.Blazor.Organizations;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class OrganizationService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrganizationService> _logger;
    private const string BaseUrl = "https://localhost:44337/api/organization/organizations";

    public OrganizationService(HttpClient httpClient, ILogger<OrganizationService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResultDto<OrganizationDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{BaseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                url += $"&Sorting={input.Sorting}";
            }

            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<OrganizationDto>>(url);
            return response ?? new PagedResultDto<OrganizationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching organizations");
            return new PagedResultDto<OrganizationDto>();
        }
    }

    public async Task<OrganizationDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<OrganizationDto>($"{BaseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching organization {Id}", id);
            return null;
        }
    }

    public async Task<OrganizationDto?> CreateAsync(CreateUpdateOrganizationDto input)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating organization");
            throw;
        }
    }

    public async Task<OrganizationDto?> UpdateAsync(Guid id, CreateUpdateOrganizationDto input)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<OrganizationDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating organization {Id}", id);
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
            _logger.LogError(ex, "Error deleting organization {Id}", id);
            throw;
        }
    }
}