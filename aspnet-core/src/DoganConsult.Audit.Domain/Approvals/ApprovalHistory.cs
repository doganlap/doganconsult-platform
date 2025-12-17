using System;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Audit.Approvals;

/// <summary>
/// Tracks history of actions on an approval request
/// </summary>
public class ApprovalHistory : CreationAuditedEntity<Guid>, IMultiTenant
{
    /// <summary>
    /// The approval request this history belongs to
    /// </summary>
    public Guid ApprovalRequestId { get; set; }

    /// <summary>
    /// The action taken (Created, Approved, Rejected, Commented, Reassigned, etc.)
    /// </summary>
    public string Action { get; set; } = string.Empty;

    /// <summary>
    /// Status before the action
    /// </summary>
    public ApprovalStatus? PreviousStatus { get; set; }

    /// <summary>
    /// Status after the action
    /// </summary>
    public ApprovalStatus NewStatus { get; set; }

    /// <summary>
    /// User who performed the action
    /// </summary>
    public Guid ActorId { get; set; }

    /// <summary>
    /// Name of the actor
    /// </summary>
    public string ActorName { get; set; } = string.Empty;

    /// <summary>
    /// Comments/notes for this action
    /// </summary>
    public string? Comments { get; set; }

    /// <summary>
    /// IP address of the actor
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User agent string
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Additional data as JSON
    /// </summary>
    public string? AdditionalData { get; set; }

    /// <summary>
    /// Tenant ID
    /// </summary>
    public Guid? TenantId { get; set; }

    protected ApprovalHistory() { }

    public ApprovalHistory(
        Guid id,
        Guid approvalRequestId,
        string action,
        ApprovalStatus newStatus,
        Guid actorId,
        string actorName,
        ApprovalStatus? previousStatus = null,
        string? comments = null,
        Guid? tenantId = null)
    {
        Id = id;
        ApprovalRequestId = approvalRequestId;
        Action = action;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ActorId = actorId;
        ActorName = actorName;
        Comments = comments;
        TenantId = tenantId;
    }
}
