using System;
using System.Diagnostics;
using System.Threading.Tasks;
using DoganConsult.AI.AIRequests;
using DoganConsult.AI.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.AI.AIRequests;

[Authorize]
public class AIAppService : ApplicationService, IAIAppService
{
    private readonly ILlmService _llmService;
    private readonly IRepository<AIRequest, Guid> _aiRequestRepository;

    public AIAppService(
        ILlmService llmService,
        IRepository<AIRequest, Guid> aiRequestRepository)
    {
        _llmService = llmService;
        _aiRequestRepository = aiRequestRepository;
    }

    [Authorize(AIPermissions.AIRequests.Create)]
    public async Task<AuditSummaryResponseDto> GenerateAuditSummaryAsync(AuditSummaryRequestDto input)
    {
        var stopwatch = Stopwatch.StartNew();
        var request = new AIRequest(
            GuidGenerator.Create(),
            "audit-summary",
            CurrentTenant.Id
        )
        {
            OrganizationId = input.OrganizationId,
            InputText = input.Text,
            Status = "pending"
        };

        await _aiRequestRepository.InsertAsync(request);

        try
        {
            var summary = await _llmService.SummarizeAsync(input.OrganizationId, input.Text);
            
            stopwatch.Stop();
            request.ResponseText = summary;
            request.Status = "completed";
            request.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            await _aiRequestRepository.UpdateAsync(request);

            return new AuditSummaryResponseDto
            {
                Summary = summary,
                RequestId = request.Id
            };
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            request.Status = "failed";
            request.ErrorMessage = ex.Message;
            request.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;

            await _aiRequestRepository.UpdateAsync(request);

            Logger.LogError(ex, "Failed to generate audit summary for organization {OrganizationId}", input.OrganizationId);
            throw;
        }
    }
}
