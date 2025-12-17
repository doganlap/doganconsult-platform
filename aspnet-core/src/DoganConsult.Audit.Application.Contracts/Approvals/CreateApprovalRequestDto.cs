using System;
using System.ComponentModel.DataAnnotations;
using DoganConsult.Audit.Workflow;

namespace DoganConsult.Audit.Approvals;

public class CreateApprovalRequestDto
{
    [Required]
    public ApprovalEntityType EntityType { get; set; }

    [Required]
    public Guid EntityId { get; set; }

    [Required]
    [StringLength(256)]
    public string EntityName { get; set; } = string.Empty;

    [Required]
    [StringLength(64)]
    public string RequestedAction { get; set; } = "Create";

    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;

    [StringLength(2000)]
    public string? RequestReason { get; set; }

    public Guid? AssignedApproverId { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public string? EntitySnapshotBefore { get; set; }

    public string? EntitySnapshotAfter { get; set; }

    public Guid? OrganizationId { get; set; }

    public int TotalWorkflowSteps { get; set; } = 1;

    public string? Metadata { get; set; }
}
