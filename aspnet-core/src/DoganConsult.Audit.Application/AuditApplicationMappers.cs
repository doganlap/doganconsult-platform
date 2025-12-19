using System.Collections.Generic;
using Riok.Mapperly.Abstractions;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Audit.Approvals;

namespace DoganConsult.Audit;

[Mapper]
public partial class AuditApplicationMappers
{
    [MapperIgnoreSource(nameof(AuditLogs.AuditLog.ExtraProperties))]
    [MapperIgnoreSource(nameof(AuditLogs.AuditLog.ConcurrencyStamp))]
    public partial AuditLogDto ToAuditLogDto(AuditLogs.AuditLog source);
    
    public partial List<AuditLogDto> ToAuditLogDtoList(List<AuditLogs.AuditLog> source);
    
    [MapperIgnoreSource(nameof(Approvals.ApprovalRequest.ExtraProperties))]
    [MapperIgnoreSource(nameof(Approvals.ApprovalRequest.ConcurrencyStamp))]
    public partial ApprovalRequestDto ToApprovalRequestDto(Approvals.ApprovalRequest source);
    
    public partial List<ApprovalRequestDto> ToApprovalRequestDtoList(List<Approvals.ApprovalRequest> source);
    
    public partial ApprovalHistoryDto ToApprovalHistoryDto(Approvals.ApprovalHistory source);
    
    public partial List<ApprovalHistoryDto> ToApprovalHistoryDtoList(List<Approvals.ApprovalHistory> source);
}
