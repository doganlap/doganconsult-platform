using System.Threading.Tasks;

namespace DoganConsult.AI.AIRequests;

public interface IAIAppService
{
    Task<AuditSummaryResponseDto> GenerateAuditSummaryAsync(AuditSummaryRequestDto input);
}
