using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using DoganConsult.AI.AIRequests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.AI.Infrastructure;

public class LlmService : ILlmService, ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<LlmService> _logger;
    private readonly string _endpoint;
    private readonly string? _apiKey;
    private readonly string? _modelName;
    private readonly int _timeoutSeconds;
    private readonly int _maxRetries;

    public LlmService(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<LlmService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _configuration = configuration;
        _logger = logger;

        var aiConfig = _configuration.GetSection("AIService");
        _endpoint = aiConfig["LlmServerEndpoint"] ?? throw new InvalidOperationException("AIService:LlmServerEndpoint is not configured");
        _apiKey = aiConfig["ApiKey"];
        _modelName = aiConfig["ModelName"];
        _timeoutSeconds = int.Parse(aiConfig["TimeoutSeconds"] ?? "30");
        _maxRetries = int.Parse(aiConfig["MaxRetries"] ?? "3");

        _httpClient.Timeout = TimeSpan.FromSeconds(_timeoutSeconds);
        
        if (!string.IsNullOrEmpty(_apiKey))
        {
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
        }
    }

    public async Task<string> SummarizeAsync(Guid organizationId, string text)
    {
        var systemPrompt = $"You are an enterprise GRC audit assistant. Summarize the following findings for organization {organizationId} in clear, concise bullet points.";
        return await CallLlmAsync(systemPrompt, text, _modelName);
    }

    public async Task<string> AnalyzeAsync(string text, string? modelName = null)
    {
        var systemPrompt = "You are an enterprise GRC document analysis assistant. Analyze the following document and provide key insights, risks, and compliance considerations.";
        return await CallLlmAsync(systemPrompt, text, modelName ?? _modelName);
    }

    private async Task<string> CallLlmAsync(string systemPrompt, string userText, string? modelName)
    {
        var requestBody = new
        {
            model = modelName ?? "default",
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user", content = userText }
            },
            temperature = 0.7,
            max_tokens = 2000
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        Exception? lastException = null;
        for (int attempt = 1; attempt <= _maxRetries; attempt++)
        {
            try
            {
                _logger.LogInformation("Calling LLM server at {Endpoint}, attempt {Attempt}", _endpoint, attempt);
                
                var response = await _httpClient.PostAsync(_endpoint, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseContent);

                // Extract the response text from the LLM response
                // Adjust this based on your hertze LLM server response format
                if (responseJson.RootElement.TryGetProperty("choices", out var choices) &&
                    choices.GetArrayLength() > 0 &&
                    choices[0].TryGetProperty("message", out var message) &&
                    message.TryGetProperty("content", out var contentElement))
                {
                    return contentElement.GetString() ?? "No response content";
                }

                // Fallback: return raw response if structure is different
                return responseContent;
            }
            catch (Exception ex)
            {
                lastException = ex;
                _logger.LogWarning(ex, "LLM call attempt {Attempt} failed", attempt);
                
                if (attempt < _maxRetries)
                {
                    await Task.Delay(1000 * attempt); // Exponential backoff
                }
            }
        }

        _logger.LogError(lastException, "All LLM call attempts failed");
        throw new InvalidOperationException($"Failed to call LLM server after {_maxRetries} attempts", lastException);
    }
}
