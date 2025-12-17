using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DoganConsult.Audit.Approvals;
using DoganConsult.Audit.Workflow;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Audit.Controllers;

[Area("audit")]
[Route("api/audit/approvals")]
public class ApprovalController : AbpControllerBase
{
    private readonly IApprovalService _approvalService;

    public ApprovalController(IApprovalService approvalService)
    {
        _approvalService = approvalService;
    }

    /// <summary>
    /// Get an approval request by ID
    /// </summary>
    [HttpGet("{id}")]
    public Task<ApprovalRequestDto> GetAsync(Guid id)
    {
        return _approvalService.GetAsync(id);
    }

    /// <summary>
    /// Get an approval request by request number
    /// </summary>
    [HttpGet("by-number/{requestNumber}")]
    public Task<ApprovalRequestDto?> GetByRequestNumberAsync(string requestNumber)
    {
        return _approvalService.GetByRequestNumberAsync(requestNumber);
    }

    /// <summary>
    /// Get paginated list of approval requests with filtering
    /// </summary>
    [HttpGet]
    public Task<PagedResultDto<ApprovalRequestDto>> GetListAsync([FromQuery] GetApprovalRequestListDto input)
    {
        return _approvalService.GetListAsync(input);
    }

    /// <summary>
    /// Get pending approval requests assigned to current user
    /// </summary>
    [HttpGet("my-pending")]
    public Task<PagedResultDto<ApprovalRequestDto>> GetMyPendingApprovalsAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _approvalService.GetMyPendingApprovalsAsync(input);
    }

    /// <summary>
    /// Get approval requests created by current user
    /// </summary>
    [HttpGet("my-requests")]
    public Task<PagedResultDto<ApprovalRequestDto>> GetMyRequestsAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _approvalService.GetMyRequestsAsync(input);
    }

    /// <summary>
    /// Create a new approval request
    /// </summary>
    [HttpPost]
    public Task<ApprovalRequestDto> CreateAsync([FromBody] CreateApprovalRequestDto input)
    {
        return _approvalService.CreateAsync(input);
    }

    /// <summary>
    /// Submit an entity for approval
    /// </summary>
    [HttpPost("submit")]
    public Task<ApprovalRequestDto> SubmitForApprovalAsync([FromBody] SubmitForApprovalRequestDto input)
    {
        return _approvalService.SubmitForApprovalAsync(
            input.EntityType,
            input.EntityId,
            input.EntityName,
            input.RequestedAction ?? "Create",
            input.Reason,
            input.AssignedApproverId);
    }

    /// <summary>
    /// Approve an approval request
    /// </summary>
    [HttpPut("{id}/approve")]
    public Task<ApprovalRequestDto> ApproveAsync(Guid id, [FromBody] ProcessApprovalCommentDto? input = null)
    {
        return _approvalService.ApproveAsync(id, input?.Comments);
    }

    /// <summary>
    /// Reject an approval request
    /// </summary>
    [HttpPut("{id}/reject")]
    public Task<ApprovalRequestDto> RejectAsync(Guid id, [FromBody] ProcessApprovalCommentDto? input = null)
    {
        return _approvalService.RejectAsync(id, input?.Comments);
    }

    /// <summary>
    /// Cancel an approval request (requester only)
    /// </summary>
    [HttpPut("{id}/cancel")]
    public Task<ApprovalRequestDto> CancelAsync(Guid id)
    {
        return _approvalService.CancelAsync(id);
    }

    /// <summary>
    /// Reassign an approval request to another approver
    /// </summary>
    [HttpPut("{id}/reassign")]
    public Task<ApprovalRequestDto> ReassignAsync(Guid id, [FromBody] ReassignApprovalDto input)
    {
        return _approvalService.ReassignAsync(id, input.NewApproverId, input.NewApproverName);
    }

    /// <summary>
    /// Get approval history for a request
    /// </summary>
    [HttpGet("{approvalRequestId}/history")]
    public Task<List<ApprovalHistoryDto>> GetHistoryAsync(Guid approvalRequestId)
    {
        return _approvalService.GetHistoryAsync(approvalRequestId);
    }

    /// <summary>
    /// Get count of pending approvals for current user
    /// </summary>
    [HttpGet("pending-count")]
    public Task<int> GetPendingCountAsync()
    {
        return _approvalService.GetPendingCountAsync();
    }

    /// <summary>
    /// Get approval statistics
    /// </summary>
    [HttpGet("statistics")]
    public Task<ApprovalStatisticsDto> GetStatisticsAsync()
    {
        return _approvalService.GetStatisticsAsync();
    }

    /// <summary>
    /// Check if an entity has pending approvals
    /// </summary>
    [HttpGet("has-pending")]
    public Task<bool> HasPendingApprovalAsync([FromQuery] ApprovalEntityType entityType, [FromQuery] Guid entityId)
    {
        return _approvalService.HasPendingApprovalAsync(entityType, entityId);
    }

    /// <summary>
    /// Get the latest approval request for an entity
    /// </summary>
    [HttpGet("latest")]
    public Task<ApprovalRequestDto?> GetLatestForEntityAsync([FromQuery] ApprovalEntityType entityType, [FromQuery] Guid entityId)
    {
        return _approvalService.GetLatestForEntityAsync(entityType, entityId);
    }
}

/// <summary>
/// DTO for submitting entities for approval
/// </summary>
public class SubmitForApprovalRequestDto
{
    public ApprovalEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string? RequestedAction { get; set; }
    public string? Reason { get; set; }
    public Guid? AssignedApproverId { get; set; }
}

/// <summary>
/// DTO for approval comments
/// </summary>
public class ProcessApprovalCommentDto
{
    public string? Comments { get; set; }
}

/// <summary>
/// DTO for reassigning approval requests
/// </summary>
public class ReassignApprovalDto
{
    public Guid NewApproverId { get; set; }
    public string NewApproverName { get; set; } = string.Empty;
}
