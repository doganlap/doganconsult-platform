using System;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Audit.Approvals;

/// <summary>
/// Represents an approval request for any entity in the system
/// </summary>
public class ApprovalRequest : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    /// <summary>
    /// Unique request number for tracking
    /// </summary>
    public string RequestNumber { get; set; } = string.Empty;

    /// <summary>
    /// Type of entity being approved
    /// </summary>
    public ApprovalEntityType EntityType { get; set; }

    /// <summary>
    /// The ID of the entity being approved
    /// </summary>
    public Guid EntityId { get; set; }

    /// <summary>
    /// Name/title of the entity for display
    /// </summary>
    public string EntityName { get; set; } = string.Empty;

    /// <summary>
    /// Current status of the approval request
    /// </summary>
    public ApprovalStatus Status { get; set; } = ApprovalStatus.Pending;

    /// <summary>
    /// Priority of the request
    /// </summary>
    public ApprovalPriority Priority { get; set; } = ApprovalPriority.Normal;

    /// <summary>
    /// User who submitted the request
    /// </summary>
    public Guid RequesterId { get; set; }

    /// <summary>
    /// Name of the requester for display
    /// </summary>
    public string RequesterName { get; set; } = string.Empty;

    /// <summary>
    /// Email of the requester
    /// </summary>
    public string? RequesterEmail { get; set; }

    /// <summary>
    /// User assigned to approve the request (null = any approver)
    /// </summary>
    public Guid? AssignedApproverId { get; set; }

    /// <summary>
    /// Name of assigned approver
    /// </summary>
    public string? AssignedApproverName { get; set; }

    /// <summary>
    /// User who actually approved/rejected the request
    /// </summary>
    public Guid? ApprovedById { get; set; }

    /// <summary>
    /// Name of the approver
    /// </summary>
    public string? ApprovedByName { get; set; }

    /// <summary>
    /// When the approval/rejection happened
    /// </summary>
    public DateTime? ApprovedAt { get; set; }

    /// <summary>
    /// Reason/justification for the request
    /// </summary>
    public string? RequestReason { get; set; }

    /// <summary>
    /// Comments from the approver
    /// </summary>
    public string? ApprovalComments { get; set; }

    /// <summary>
    /// When the request expires (optional)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// The action being requested (Create, Update, Delete, Activate, etc.)
    /// </summary>
    public string RequestedAction { get; set; } = "Create";

    /// <summary>
    /// JSON snapshot of the entity before changes (for updates)
    /// </summary>
    public string? EntitySnapshotBefore { get; set; }

    /// <summary>
    /// JSON snapshot of the entity after changes (for updates)
    /// </summary>
    public string? EntitySnapshotAfter { get; set; }

    /// <summary>
    /// Organization context for the request
    /// </summary>
    public Guid? OrganizationId { get; set; }

    /// <summary>
    /// Optional workflow stage/step
    /// </summary>
    public int WorkflowStep { get; set; } = 1;

    /// <summary>
    /// Total steps in the workflow
    /// </summary>
    public int TotalWorkflowSteps { get; set; } = 1;

    /// <summary>
    /// Additional metadata as JSON
    /// </summary>
    public string? Metadata { get; set; }

    /// <summary>
    /// Tenant ID for multi-tenancy
    /// </summary>
    public Guid? TenantId { get; set; }

    protected ApprovalRequest() { }

    public ApprovalRequest(
        Guid id,
        ApprovalEntityType entityType,
        Guid entityId,
        string entityName,
        Guid requesterId,
        string requesterName,
        string requestedAction = "Create",
        Guid? tenantId = null)
    {
        Id = id;
        RequestNumber = GenerateRequestNumber();
        EntityType = entityType;
        EntityId = entityId;
        EntityName = entityName;
        RequesterId = requesterId;
        RequesterName = requesterName;
        RequestedAction = requestedAction;
        Status = ApprovalStatus.Pending;
        TenantId = tenantId;
    }

    private static string GenerateRequestNumber()
    {
        return $"APR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
    }

    public void Approve(Guid approverId, string approverName, string? comments = null)
    {
        Status = ApprovalStatus.Approved;
        ApprovedById = approverId;
        ApprovedByName = approverName;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
    }

    public void Reject(Guid approverId, string approverName, string? comments = null)
    {
        Status = ApprovalStatus.Rejected;
        ApprovedById = approverId;
        ApprovedByName = approverName;
        ApprovedAt = DateTime.UtcNow;
        ApprovalComments = comments;
    }

    public void Cancel()
    {
        Status = ApprovalStatus.Cancelled;
    }

    public void Expire()
    {
        if (Status == ApprovalStatus.Pending)
        {
            Status = ApprovalStatus.Expired;
        }
    }
}
