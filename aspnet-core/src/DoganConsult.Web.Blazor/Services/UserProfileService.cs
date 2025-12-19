using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.UserProfile.UserProfiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class UserProfileService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<UserProfileService> _logger;
    private readonly string _baseUrl;

    public UserProfileService(HttpClient httpClient, ILogger<UserProfileService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        var gatewayBaseUrl = configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        _baseUrl = $"{gatewayBaseUrl.TrimEnd('/')}/api/userprofile/user-profiles";
    }

    public async Task<PagedResultDto<UserProfileDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{_baseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                url += $"&Sorting={input.Sorting}";
            }
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<UserProfileDto>>(url);
            return response ?? new PagedResultDto<UserProfileDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profiles");
            return new PagedResultDto<UserProfileDto>();
        }
    }

    public async Task<UserProfileDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<UserProfileDto>($"{_baseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile {Id}", id);
            return null;
        }
    }

    public async Task<UserProfileDto?> CreateAsync(CreateUpdateUserProfileDto input)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserProfileDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user profile");
            throw;
        }
    }

    public async Task<UserProfileDto?> UpdateAsync(Guid id, CreateUpdateUserProfileDto input)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<UserProfileDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile {Id}", id);
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
            _logger.LogError(ex, "Error deleting user profile {Id}", id);
            throw;
        }
    }
}
