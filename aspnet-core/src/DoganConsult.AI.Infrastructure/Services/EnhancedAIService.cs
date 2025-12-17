using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.DependencyInjection;
using DoganConsult.AI.AIRequests;
using DoganConsult.AI.Application.Contracts.Services;
using DoganConsult.AI.Infrastructure.Configuration;
using DoganConsult.AI.Infrastructure.Factories;

namespace DoganConsult.AI.Infrastructure.Services;

public class EnhancedAIService : IEnhancedAIService, ITransientDependency
{
    private readonly SimpleAIAgent _consultingAgent;
    private readonly SimpleAIAgent _auditAgent;
    private readonly SimpleAIAgent _complianceAgent;
    private readonly IRepository<AIRequest> _aiRequestRepository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<EnhancedAIService> _logger;
    private readonly AgentConfiguration _config;
    private readonly Dictionary<string, string> _activeThreads;

    public EnhancedAIService(
        AIAgentFactory agentFactory,
        IRepository<AIRequest> aiRequestRepository,
        IDistributedCache cache,
        ILogger<EnhancedAIService> logger,
        IOptions<AgentConfiguration> config)
    {
        _consultingAgent = agentFactory.CreateConsultingAgent();
        _auditAgent = agentFactory.CreateAuditAgent();
        _complianceAgent = agentFactory.CreateComplianceAgent();
        _aiRequestRepository = aiRequestRepository;
        _cache = cache;
        _logger = logger;
        _config = config.Value;
        _activeThreads = new Dictionary<string, string>();
    }

    public async Task<string> GenerateAuditSummaryAsync(string auditData)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var prompt = $@"
            Generate a comprehensive audit summary for the following data:
            
            {auditData}
            
            Please provide:
            1. Executive Summary
            2. Key Findings
            3. Risk Assessment
            4. Recommendations
            5. Next Steps
            
            Format as professional audit report.";

            var response = await _auditAgent.RunAsync(prompt);
            
            await LogAIRequestAsync("AuditSummary", prompt, response, startTime);
            
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating audit summary");
            await LogAIRequestAsync("AuditSummary", auditData, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<ChatResponseDto> ChatAsync(ChatRequestDto request)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var agent = GetAgentForContext(request.Context);
            var threadId = !string.IsNullOrEmpty(request.ThreadId) ? request.ThreadId : agent.GetNewThreadId();
            
            var response = await agent.RunAsync(request.Message, threadId);

            var chatResponse = new ChatResponseDto
            {
                Response = response,
                ThreadId = threadId,
                Timestamp = DateTime.UtcNow,
                EstimatedCost = EstimateCost(request.Message, response),
                ResponseTimeMs = (int)(DateTime.UtcNow - startTime).TotalMilliseconds
            };

            _activeThreads[threadId] = request.Message;

            await LogAIRequestAsync("Chat", request.Message, response, startTime, threadId);
            
            return chatResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in chat conversation");
            await LogAIRequestAsync("Chat", request.Message, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<DocumentAnalysisResultDto> AnalyzeDocumentAsync(DocumentAnalysisRequestDto request)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var analysisPrompt = $@"
            Analyze the following document and provide a structured analysis:
            
            Document Type: {request.DocumentType}
            Analysis Types: {string.Join(", ", request.AnalysisTypes)}
            
            Content:
            {request.Content}
            
            Provide analysis with key insights, compliance indicators, risk factors, and recommendations.";

            var response = await _consultingAgent.RunAsync(analysisPrompt);
            
            await LogAIRequestAsync("DocumentAnalysis", analysisPrompt, response, startTime);

            var result = new DocumentAnalysisResultDto
            {
                Summary = response,
                ConfidenceScore = 0.85m,
                KeyInsights = ExtractKeyInsights(response),
                ComplianceIndicators = ExtractComplianceIndicators(response),
                RiskFactors = ExtractRiskFactors(response),
                Recommendations = ExtractRecommendations(response)
            };
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing document");
            await LogAIRequestAsync("DocumentAnalysis", request.Content, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<ComplianceCheckResultDto> CheckComplianceAsync(ComplianceCheckRequestDto request)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var compliancePrompt = $@"
            Perform compliance check for:
            Organization: {request.OrganizationType}
            Sector: {request.Sector}
            Regulations: {string.Join(", ", request.ApplicableRegulations)}
            
            Data to check: {request.DataToCheck}
            
            Provide compliance assessment including status, violations, and recommendations.";

            var response = await _complianceAgent.RunAsync(compliancePrompt);
            
            await LogAIRequestAsync("ComplianceCheck", compliancePrompt, response, startTime);

            var result = new ComplianceCheckResultDto
            {
                Status = "NeedsReview",
                Summary = response,
                RiskLevel = "Medium",
                Violations = new List<ComplianceViolationDto>(),
                Recommendations = new List<ComplianceRecommendationDto>()
            };
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking compliance");
            await LogAIRequestAsync("ComplianceCheck", request.DataToCheck, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<RiskAssessmentResultDto> AssessRiskAsync(RiskAssessmentRequestDto request)
    {
        var startTime = DateTime.UtcNow;
        
        try
        {
            var riskPrompt = $@"
            Perform risk assessment for:
            Category: {request.RiskCategory}
            Description: {request.Description}
            Risk Factors: {JsonSerializer.Serialize(request.RiskFactors)}
            Organization Context: {request.OrganizationContext}
            
            Provide comprehensive risk assessment with score, level, and mitigation strategies.";

            var response = await _auditAgent.RunAsync(riskPrompt);
            
            await LogAIRequestAsync("RiskAssessment", riskPrompt, response, startTime);

            var result = new RiskAssessmentResultDto
            {
                OverallRiskScore = CalculateRiskScore(request.RiskFactors),
                RiskLevel = DetermineRiskLevel(request.RiskFactors),
                Assessment = response,
                IdentifiedRisks = new List<RiskFactorDto>(),
                MitigationStrategies = new List<MitigationStrategyDto>()
            };
            
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assessing risk");
            await LogAIRequestAsync("RiskAssessment", request.Description, $"Error: {ex.Message}", startTime);
            throw;
        }
    }

    public async Task<string> CreateConversationThreadAsync(Guid userId, Guid? tenantId = null)
    {
        var threadId = _consultingAgent.GetNewThreadId();
        
        _activeThreads[threadId] = $"User: {userId}, Tenant: {tenantId}";
        
        await LogAIRequestAsync("CreateThread", $"User: {userId}, Tenant: {tenantId}", threadId, DateTime.UtcNow, threadId);
        
        return threadId;
    }

    public async Task<ChatResponseDto> ContinueConversationAsync(string threadId, string message)
    {
        if (!_activeThreads.ContainsKey(threadId))
        {
            throw new ArgumentException($"Thread {threadId} not found");
        }

        var startTime = DateTime.UtcNow;
        
        try
        {
            var response = await _consultingAgent.RunAsync(message, threadId);
            
            var chatResponse = new ChatResponseDto
            {
                Response = response,
                ThreadId = threadId,
                Timestamp = DateTime.UtcNow,
                EstimatedCost = EstimateCost(message, response),
                ResponseTimeMs = (int)(DateTime.UtcNow - startTime).TotalMilliseconds
            };

            await LogAIRequestAsync("ContinueConversation", message, response, startTime, threadId);
            
            return chatResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error continuing conversation");
            await LogAIRequestAsync("ContinueConversation", message, $"Error: {ex.Message}", startTime, threadId);
            throw;
        }
    }

    #region Private Helper Methods

    private SimpleAIAgent GetAgentForContext(Dictionary<string, string>? context)
    {
        if (context == null) return _consultingAgent;

        if (context.ContainsKey("domain"))
        {
            var domain = context["domain"].ToLower();
            return domain switch
            {
                "audit" => _auditAgent,
                "compliance" => _complianceAgent,
                "consulting" => _consultingAgent,
                _ => _consultingAgent
            };
        }

        return _consultingAgent;
    }

    private decimal EstimateCost(string input, string output)
    {
        var inputTokens = input.Length / 4;
        var outputTokens = output.Length / 4;
        var inputCost = inputTokens * 0.000003m;
        var outputCost = outputTokens * 0.000015m;
        return inputCost + outputCost;
    }

    private decimal CalculateRiskScore(Dictionary<string, decimal> riskFactors)
    {
        if (!riskFactors.Any()) return 5.0m;
        return riskFactors.Values.Average();
    }

    private string DetermineRiskLevel(Dictionary<string, decimal> riskFactors)
    {
        var avgScore = CalculateRiskScore(riskFactors);
        return avgScore switch
        {
            < 3.0m => "Low",
            < 6.0m => "Medium",
            < 8.0m => "High",
            _ => "Critical"
        };
    }

    private List<KeyInsightDto> ExtractKeyInsights(string response)
    {
        return new List<KeyInsightDto>
        {
            new() { Title = "Analysis Result", Description = response, Confidence = 0.8m, Category = "General" }
        };
    }

    private List<ComplianceIndicatorDto> ExtractComplianceIndicators(string response)
    {
        return new List<ComplianceIndicatorDto>();
    }

    private List<RiskFactorDto> ExtractRiskFactors(string response)
    {
        return new List<RiskFactorDto>();
    }

    private List<RecommendationDto> ExtractRecommendations(string response)
    {
        return new List<RecommendationDto>();
    }

    private async Task LogAIRequestAsync(string requestType, string request, string response, DateTime startTime, string? threadId = null)
    {
        if (!_config.EnableRequestLogging) return;

        try
        {
            var aiRequest = new AIRequest(
                Guid.NewGuid(),
                requestType)
            {
                ModelName = "github-models",
                InputText = request.Length > 2000 ? request.Substring(0, 2000) : request,
                ResponseText = response.Length > 10000 ? response.Substring(0, 10000) : response,
                ProcessingTimeMs = (int)(DateTime.UtcNow - startTime).TotalMilliseconds,
                TokensUsed = EstimateTokens(request, response),
                Status = "completed"
            };

            await _aiRequestRepository.InsertAsync(aiRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging AI request");
        }
    }

    private int EstimateTokens(string input, string output)
    {
        return (input.Length + output.Length) / 4; // Rough estimation
    }

    #endregion
}