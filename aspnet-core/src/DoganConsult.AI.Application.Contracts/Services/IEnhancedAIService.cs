using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace DoganConsult.AI.Application.Contracts.Services;

public interface IEnhancedAIService : IApplicationService
{
    Task<string> GenerateAuditSummaryAsync(string auditData);
    Task<ChatResponseDto> ChatAsync(ChatRequestDto request);
    Task<DocumentAnalysisResultDto> AnalyzeDocumentAsync(DocumentAnalysisRequestDto request);
    Task<ComplianceCheckResultDto> CheckComplianceAsync(ComplianceCheckRequestDto request);
    Task<RiskAssessmentResultDto> AssessRiskAsync(RiskAssessmentRequestDto request);
    Task<string> CreateConversationThreadAsync(Guid userId, Guid? tenantId = null);
    Task<ChatResponseDto> ContinueConversationAsync(string threadId, string message);
}

// DTOs
public class ChatRequestDto
{
    public string Message { get; set; } = "";
    public string? ThreadId { get; set; }
    public string? ModelId { get; set; }
    public Dictionary<string, string>? Context { get; set; }
}

public class ChatResponseDto
{
    public string Response { get; set; } = "";
    public string ThreadId { get; set; } = "";
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public decimal EstimatedCost { get; set; }
    public int ResponseTimeMs { get; set; }
}

public class DocumentAnalysisRequestDto
{
    public string Content { get; set; } = "";
    public string DocumentType { get; set; } = "";
    public List<string> AnalysisTypes { get; set; } = new();
}

public class DocumentAnalysisResultDto
{
    public List<KeyInsightDto> KeyInsights { get; set; } = new();
    public List<ComplianceIndicatorDto> ComplianceIndicators { get; set; } = new();
    public List<RiskFactorDto> RiskFactors { get; set; } = new();
    public List<RecommendationDto> Recommendations { get; set; } = new();
    public string Summary { get; set; } = "";
    public decimal ConfidenceScore { get; set; }
}

public class ComplianceCheckRequestDto
{
    public string OrganizationType { get; set; } = "";
    public string Sector { get; set; } = "";
    public List<string> ApplicableRegulations { get; set; } = new();
    public string DataToCheck { get; set; } = "";
}

public class ComplianceCheckResultDto
{
    public string Status { get; set; } = "";
    public List<ComplianceViolationDto> Violations { get; set; } = new();
    public List<ComplianceRecommendationDto> Recommendations { get; set; } = new();
    public string RiskLevel { get; set; } = "";
    public string Summary { get; set; } = "";
}

public class RiskAssessmentRequestDto
{
    public string RiskCategory { get; set; } = "";
    public string Description { get; set; } = "";
    public Dictionary<string, decimal> RiskFactors { get; set; } = new();
    public string OrganizationContext { get; set; } = "";
}

public class RiskAssessmentResultDto
{
    public decimal OverallRiskScore { get; set; }
    public string RiskLevel { get; set; } = "";
    public List<RiskFactorDto> IdentifiedRisks { get; set; } = new();
    public List<MitigationStrategyDto> MitigationStrategies { get; set; } = new();
    public string Assessment { get; set; } = "";
}

// Supporting DTOs
public class KeyInsightDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Confidence { get; set; }
    public string Category { get; set; } = "";
}

public class ComplianceIndicatorDto
{
    public string Regulation { get; set; } = "";
    public string Status { get; set; } = "";
    public string Details { get; set; } = "";
    public List<string> RequiredActions { get; set; } = new();
}

public class RiskFactorDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Level { get; set; } = "";
    public decimal Score { get; set; }
    public string Category { get; set; } = "";
}

public class RecommendationDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "";
    public string Category { get; set; } = "";
    public int EstimatedImplementationHours { get; set; }
}

public class ComplianceViolationDto
{
    public string Regulation { get; set; } = "";
    public string ViolationType { get; set; } = "";
    public string Description { get; set; } = "";
    public string Severity { get; set; } = "";
    public List<string> RequiredActions { get; set; } = new();
}

public class ComplianceRecommendationDto
{
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "";
    public int ExpectedImplementationHours { get; set; }
}

public class MitigationStrategyDto
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Priority { get; set; } = "";
    public decimal EffectivenessScore { get; set; }
    public int ImplementationHours { get; set; }
}