using System.Collections.Generic;
using DoganConsult.Audit.Approvals;
using Riok.Mapperly.Abstractions;

namespace DoganConsult.Audit;

/// <summary>
/// Mapperly mapper for Audit entities
/// </summary>
[Mapper]
public partial class AuditMapper
{
    [MapperIgnoreSource(nameof(ApprovalRequest.ExtraProperties))]
    [MapperIgnoreSource(nameof(ApprovalRequest.ConcurrencyStamp))]
    public partial ApprovalRequestDto Map(ApprovalRequest source);
    
    [MapperIgnoreSource(nameof(ApprovalHistory.UserAgent))]
    public partial ApprovalHistoryDto Map(ApprovalHistory source);
    
    public partial List<ApprovalRequestDto> Map(List<ApprovalRequest> source);
    public partial List<ApprovalHistoryDto> Map(List<ApprovalHistory> source);
}
