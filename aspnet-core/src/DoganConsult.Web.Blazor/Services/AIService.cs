using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DoganConsult.AI.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class AIService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AIService> _logger;
    private const string BaseUrl = "https://localhost:44331/api/ai";

    public AIService(HttpClient httpClient, ILogger<AIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<ChatResponseDto?> ChatAsync(ChatRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/chat", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending chat message");
            throw;
        }
    }

    public async Task<string?> CreateConversationThreadAsync(Guid userId)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/thread", new { UserId = userId });
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();
            return result.Trim('"');
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating conversation thread");
            throw;
        }
    }

    public async Task<ChatResponseDto?> ContinueConversationAsync(string threadId, string message)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/thread/{threadId}/continue", new { Message = message });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ChatResponseDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error continuing conversation");
            throw;
        }
    }

    public async Task<DocumentAnalysisResultDto?> AnalyzeDocumentAsync(DocumentAnalysisRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/analyze-document", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DocumentAnalysisResultDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing document");
            throw;
        }
    }

    public async Task<ComplianceCheckResultDto?> CheckComplianceAsync(ComplianceCheckRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/compliance-check", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ComplianceCheckResultDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking compliance");
            throw;
        }
    }

    public async Task<RiskAssessmentResultDto?> AssessRiskAsync(RiskAssessmentRequestDto request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/risk-assessment", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<RiskAssessmentResultDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing risk");
            throw;
        }
    }

    public async Task<string?> GenerateAuditSummaryAsync(string auditData)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/audit-summary", new { AuditData = auditData });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating audit summary");
            throw;
        }
    }
}
