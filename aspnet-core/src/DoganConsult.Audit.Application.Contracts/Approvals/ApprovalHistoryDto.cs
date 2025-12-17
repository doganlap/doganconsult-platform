using System;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Audit.Approvals;

public class ApprovalHistoryDto : CreationAuditedEntityDto<Guid>
{
    public Guid ApprovalRequestId { get; set; }
    public string Action { get; set; } = string.Empty;
    public ApprovalStatus? PreviousStatus { get; set; }
    public ApprovalStatus NewStatus { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public string? IpAddress { get; set; }
    public string? AdditionalData { get; set; }
    public Guid? TenantId { get; set; }

    public string StatusChangeDisplay => PreviousStatus.HasValue
        ? $"{PreviousStatus} → {NewStatus}"
        : NewStatus.ToString();
}
