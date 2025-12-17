using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoganConsult.Audit.Workflow;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.Audit.Approvals;

public interface IApprovalService : IApplicationService
{
    /// <summary>
    /// Get a single approval request by ID
    /// </summary>
    Task<ApprovalRequestDto> GetAsync(Guid id);

    /// <summary>
    /// Get an approval request by request number
    /// </summary>
    Task<ApprovalRequestDto?> GetByRequestNumberAsync(string requestNumber);

    /// <summary>
    /// Get a paged list of approval requests with filtering
    /// </summary>
    Task<PagedResultDto<ApprovalRequestDto>> GetListAsync(GetApprovalRequestListDto input);

    /// <summary>
    /// Get pending approval requests for the current user
    /// </summary>
    Task<PagedResultDto<ApprovalRequestDto>> GetMyPendingApprovalsAsync(PagedAndSortedResultRequestDto input);

    /// <summary>
    /// Get approval requests submitted by the current user
    /// </summary>
    Task<PagedResultDto<ApprovalRequestDto>> GetMyRequestsAsync(PagedAndSortedResultRequestDto input);

    /// <summary>
    /// Create a new approval request
    /// </summary>
    Task<ApprovalRequestDto> CreateAsync(CreateApprovalRequestDto input);

    /// <summary>
    /// Submit an entity for approval
    /// </summary>
    Task<ApprovalRequestDto> SubmitForApprovalAsync(
        ApprovalEntityType entityType,
        Guid entityId,
        string entityName,
        string requestedAction = "Create",
        string? reason = null,
        Guid? assignedApproverId = null);

    /// <summary>
    /// Approve a request
    /// </summary>
    Task<ApprovalRequestDto> ApproveAsync(Guid id, string? comments = null);

    /// <summary>
    /// Reject a request
    /// </summary>
    Task<ApprovalRequestDto> RejectAsync(Guid id, string? comments = null);

    /// <summary>
    /// Cancel a request (by requester)
    /// </summary>
    Task<ApprovalRequestDto> CancelAsync(Guid id);

    /// <summary>
    /// Reassign approval to a different approver
    /// </summary>
    Task<ApprovalRequestDto> ReassignAsync(Guid id, Guid newApproverId, string newApproverName);

    /// <summary>
    /// Get history for an approval request
    /// </summary>
    Task<List<ApprovalHistoryDto>> GetHistoryAsync(Guid approvalRequestId);

    /// <summary>
    /// Get count of pending approvals for the current user
    /// </summary>
    Task<int> GetPendingCountAsync();

    /// <summary>
    /// Get approval statistics
    /// </summary>
    Task<ApprovalStatisticsDto> GetStatisticsAsync();

    /// <summary>
    /// Check if an entity has a pending approval
    /// </summary>
    Task<bool> HasPendingApprovalAsync(ApprovalEntityType entityType, Guid entityId);

    /// <summary>
    /// Get the latest approval request for an entity
    /// </summary>
    Task<ApprovalRequestDto?> GetLatestForEntityAsync(ApprovalEntityType entityType, Guid entityId);
}
