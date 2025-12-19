using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Document.Documents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class DocumentService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DocumentService> _logger;
    private readonly string _baseUrl;

    public DocumentService(HttpClient httpClient, ILogger<DocumentService> logger, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _logger = logger;
        var gatewayBaseUrl = configuration["RemoteServices:Default:BaseUrl"] ?? "http://localhost:5000";
        _baseUrl = $"{gatewayBaseUrl.TrimEnd('/')}/api/document/documents";
    }

    public async Task<PagedResultDto<DocumentDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{_baseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            if (!string.IsNullOrEmpty(input.Sorting))
            {
                url += $"&Sorting={input.Sorting}";
            }
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<DocumentDto>>(url);
            return response ?? new PagedResultDto<DocumentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching documents");
            return new PagedResultDto<DocumentDto>();
        }
    }

    public async Task<DocumentDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<DocumentDto>($"{_baseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching document {Id}", id);
            return null;
        }
    }

    public async Task<DocumentDto?> CreateAsync(CreateUpdateDocumentDto input)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync(_baseUrl, input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DocumentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating document");
            throw;
        }
    }

    public async Task<DocumentDto?> UpdateAsync(Guid id, CreateUpdateDocumentDto input)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{_baseUrl}/{id}", input);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DocumentDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating document {Id}", id);
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
            _logger.LogError(ex, "Error deleting document {Id}", id);
            throw;
        }
    }
}
