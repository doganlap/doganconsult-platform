using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.Document.Documents;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class DocumentService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<DocumentService> _logger;
    private const string BaseUrl = "https://localhost:44348/api/document/documents";

    public DocumentService(HttpClient httpClient, ILogger<DocumentService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResultDto<DocumentDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        try
        {
            var url = $"{BaseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
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
            return await _httpClient.GetFromJsonAsync<DocumentDto>($"{BaseUrl}/{id}");
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
            var response = await _httpClient.PostAsJsonAsync(BaseUrl, input);
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
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}", input);
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
            var response = await _httpClient.DeleteAsync($"{BaseUrl}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting document {Id}", id);
            throw;
        }
    }
}
