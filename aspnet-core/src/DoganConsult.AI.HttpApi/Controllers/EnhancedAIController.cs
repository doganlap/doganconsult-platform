using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using DoganConsult.AI.Application.Contracts.Services;

namespace DoganConsult.AI.HttpApi.Controllers;

[RemoteService]
[Route("api/ai/enhanced")]
[ApiController]
public class EnhancedAIController : AbpControllerBase
{
    private readonly IEnhancedAIService _enhancedAIService;

    public EnhancedAIController(IEnhancedAIService enhancedAIService)
    {
        _enhancedAIService = enhancedAIService;
    }

    [HttpPost("audit-summary")]
    public async Task<string> GenerateAuditSummaryAsync([FromBody] string auditData)
    {
        return await _enhancedAIService.GenerateAuditSummaryAsync(auditData);
    }

    [HttpPost("chat")]
    public async Task<ChatResponseDto> ChatAsync([FromBody] ChatRequestDto request)
    {
        return await _enhancedAIService.ChatAsync(request);
    }

    [HttpPost("documents/analyze")]
    public async Task<DocumentAnalysisResultDto> AnalyzeDocumentAsync([FromBody] DocumentAnalysisRequestDto request)
    {
        return await _enhancedAIService.AnalyzeDocumentAsync(request);
    }

    [HttpPost("compliance/check")]
    public async Task<ComplianceCheckResultDto> CheckComplianceAsync([FromBody] ComplianceCheckRequestDto request)
    {
        return await _enhancedAIService.CheckComplianceAsync(request);
    }

    [HttpPost("risk/assess")]
    public async Task<RiskAssessmentResultDto> AssessRiskAsync([FromBody] RiskAssessmentRequestDto request)
    {
        return await _enhancedAIService.AssessRiskAsync(request);
    }

    [HttpPost("conversations")]
    public async Task<string> CreateConversationThreadAsync([FromQuery] Guid userId, [FromQuery] Guid? tenantId = null)
    {
        return await _enhancedAIService.CreateConversationThreadAsync(userId, tenantId);
    }

    [HttpPost("conversations/{threadId}/continue")]
    public async Task<ChatResponseDto> ContinueConversationAsync(string threadId, [FromBody] string message)
    {
        return await _enhancedAIService.ContinueConversationAsync(threadId, message);
    }
}