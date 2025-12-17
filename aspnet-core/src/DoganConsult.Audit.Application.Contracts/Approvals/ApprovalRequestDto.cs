using System;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Audit.Approvals;

public class ApprovalRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestNumber { get; set; } = string.Empty;
    public ApprovalEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
    public ApprovalPriority Priority { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string? RequesterEmail { get; set; }
    public Guid? AssignedApproverId { get; set; }
    public string? AssignedApproverName { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? RequestReason { get; set; }
    public string? ApprovalComments { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string RequestedAction { get; set; } = string.Empty;
    public string? EntitySnapshotBefore { get; set; }
    public string? EntitySnapshotAfter { get; set; }
    public Guid? OrganizationId { get; set; }
    public int WorkflowStep { get; set; }
    public int TotalWorkflowSteps { get; set; }
    public string? Metadata { get; set; }
    public Guid? TenantId { get; set; }

    // Computed properties for UI
    public string StatusDisplay => Status.ToString();
    public string EntityTypeDisplay => EntityType.ToString();
    public string PriorityDisplay => Priority.ToString();
    public bool CanApprove => Status == ApprovalStatus.Pending;
    public bool CanReject => Status == ApprovalStatus.Pending;
    public bool CanCancel => Status == ApprovalStatus.Pending;
}
