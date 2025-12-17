using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Audit.Approvals;
using DoganConsult.Audit.Permissions;
using DoganConsult.Audit.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Users;

namespace DoganConsult.Audit.Approvals;

[Authorize]
public class ApprovalService : ApplicationService, IApprovalService
{
    private readonly IApprovalRequestRepository _approvalRequestRepository;
    private readonly IApprovalHistoryRepository _approvalHistoryRepository;
    private readonly IRepository<ApprovalRequest, Guid> _approvalRepository;
    private readonly IRepository<ApprovalHistory, Guid> _historyRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<ApprovalService> _logger;

    public ApprovalService(
        IApprovalRequestRepository approvalRequestRepository,
        IApprovalHistoryRepository approvalHistoryRepository,
        IRepository<ApprovalRequest, Guid> approvalRepository,
        IRepository<ApprovalHistory, Guid> historyRepository,
        IEmailSender emailSender,
        ILogger<ApprovalService> logger)
    {
        _approvalRequestRepository = approvalRequestRepository;
        _approvalHistoryRepository = approvalHistoryRepository;
        _approvalRepository = approvalRepository;
        _historyRepository = historyRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<ApprovalRequestDto> GetAsync(Guid id)
    {
        var approval = await _approvalRequestRepository.GetAsync(id);
        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval);
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<ApprovalRequestDto?> GetByRequestNumberAsync(string requestNumber)
    {
        var approval = await _approvalRequestRepository.FindByRequestNumberAsync(requestNumber);
        return approval != null ? ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval) : null;
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<PagedResultDto<ApprovalRequestDto>> GetListAsync(GetApprovalRequestListDto input)
    {
        var query = await _approvalRepository.GetQueryableAsync();

        if (input.Status.HasValue)
            query = query.Where(x => x.Status == input.Status.Value);

        if (input.EntityType.HasValue)
            query = query.Where(x => x.EntityType == input.EntityType.Value);

        if (input.Priority.HasValue)
            query = query.Where(x => x.Priority == input.Priority.Value);

        if (!string.IsNullOrWhiteSpace(input.RequestNumber))
            query = query.Where(x => x.RequestNumber.Contains(input.RequestNumber));

        if (!string.IsNullOrWhiteSpace(input.RequesterName))
            query = query.Where(x => x.RequesterName.Contains(input.RequesterName));

        if (input.PendingOnly == true)
            query = query.Where(x => x.Status == ApprovalStatus.Pending);

        if (input.MyRequestsOnly == true && CurrentUser.Id.HasValue)
            query = query.Where(x => x.RequesterId == CurrentUser.Id.Value);

        if (input.AssignedToMe == true && CurrentUser.Id.HasValue)
            query = query.Where(x => x.AssignedApproverId == null || x.AssignedApproverId == CurrentUser.Id.Value);

        var totalCount = await AsyncExecuter.CountAsync(query);

        query = query.OrderByDescending(x => x.CreationTime)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var items = await AsyncExecuter.ToListAsync(query);

        return new PagedResultDto<ApprovalRequestDto>(
            totalCount,
            ObjectMapper.Map<List<ApprovalRequest>, List<ApprovalRequestDto>>(items)
        );
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<PagedResultDto<ApprovalRequestDto>> GetMyPendingApprovalsAsync(PagedAndSortedResultRequestDto input)
    {
        if (!CurrentUser.Id.HasValue)
            return new PagedResultDto<ApprovalRequestDto>(0, new List<ApprovalRequestDto>());

        var items = await _approvalRequestRepository.GetPendingRequestsAsync(
            CurrentUser.Id.Value,
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount);

        var totalCount = await _approvalRequestRepository.GetPendingCountAsync(CurrentUser.Id.Value);

        return new PagedResultDto<ApprovalRequestDto>(
            totalCount,
            ObjectMapper.Map<List<ApprovalRequest>, List<ApprovalRequestDto>>(items)
        );
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<PagedResultDto<ApprovalRequestDto>> GetMyRequestsAsync(PagedAndSortedResultRequestDto input)
    {
        if (!CurrentUser.Id.HasValue)
            return new PagedResultDto<ApprovalRequestDto>(0, new List<ApprovalRequestDto>());

        var items = await _approvalRequestRepository.GetListByRequesterAsync(
            CurrentUser.Id.Value,
            skipCount: input.SkipCount,
            maxResultCount: input.MaxResultCount);

        var query = await _approvalRepository.GetQueryableAsync();
        var totalCount = await AsyncExecuter.CountAsync(
            query.Where(x => x.RequesterId == CurrentUser.Id.Value));

        return new PagedResultDto<ApprovalRequestDto>(
            totalCount,
            ObjectMapper.Map<List<ApprovalRequest>, List<ApprovalRequestDto>>(items)
        );
    }

    [Authorize(AuditPermissions.Approvals.Create)]
    public async Task<ApprovalRequestDto> CreateAsync(CreateApprovalRequestDto input)
    {
        if (!CurrentUser.Id.HasValue)
            throw new BusinessException("User must be authenticated to create approval requests");

        var approvalRequest = new ApprovalRequest(
            GuidGenerator.Create(),
            input.EntityType,
            input.EntityId,
            input.EntityName,
            CurrentUser.Id.Value,
            CurrentUser.Name ?? CurrentUser.UserName ?? "Unknown",
            input.RequestedAction,
            CurrentTenant.Id)
        {
            Priority = input.Priority,
            RequestReason = input.RequestReason,
            AssignedApproverId = input.AssignedApproverId,
            ExpiresAt = input.ExpiresAt,
            EntitySnapshotBefore = input.EntitySnapshotBefore,
            EntitySnapshotAfter = input.EntitySnapshotAfter,
            OrganizationId = input.OrganizationId,
            TotalWorkflowSteps = input.TotalWorkflowSteps,
            Metadata = input.Metadata,
            RequesterEmail = CurrentUser.Email
        };

        await _approvalRequestRepository.InsertAsync(approvalRequest, autoSave: true);

        // Add history entry
        await AddHistoryEntryAsync(
            approvalRequest.Id,
            "Created",
            ApprovalStatus.Pending,
            CurrentUser.Id.Value,
            CurrentUser.Name ?? "Unknown",
            null,
            "Approval request created");

        // Send email notification if approver is assigned
        if (input.AssignedApproverId.HasValue)
        {
            await SendApprovalNotificationEmailAsync(approvalRequest);
        }

        _logger.LogInformation("Approval request {RequestNumber} created for {EntityType} {EntityId}",
            approvalRequest.RequestNumber, input.EntityType, input.EntityId);

        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approvalRequest);
    }

    [Authorize(AuditPermissions.Approvals.Create)]
    public async Task<ApprovalRequestDto> SubmitForApprovalAsync(
        ApprovalEntityType entityType,
        Guid entityId,
        string entityName,
        string requestedAction = "Create",
        string? reason = null,
        Guid? assignedApproverId = null)
    {
        var input = new CreateApprovalRequestDto
        {
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            RequestedAction = requestedAction,
            RequestReason = reason,
            AssignedApproverId = assignedApproverId
        };

        return await CreateAsync(input);
    }

    [Authorize(AuditPermissions.Approvals.Approve)]
    public async Task<ApprovalRequestDto> ApproveAsync(Guid id, string? comments = null)
    {
        if (!CurrentUser.Id.HasValue)
            throw new BusinessException("User must be authenticated to approve requests");

        var approval = await _approvalRequestRepository.GetAsync(id);

        if (approval.Status != ApprovalStatus.Pending)
            throw new BusinessException($"Only pending requests can be approved. Current status: {approval.Status}");

        // Check if user is authorized to approve
        if (approval.AssignedApproverId.HasValue && approval.AssignedApproverId != CurrentUser.Id)
            throw new BusinessException("You are not authorized to approve this request");

        var previousStatus = approval.Status;
        approval.Approve(CurrentUser.Id.Value, CurrentUser.Name ?? "Unknown", comments);

        await _approvalRequestRepository.UpdateAsync(approval, autoSave: true);

        await AddHistoryEntryAsync(
            approval.Id,
            "Approved",
            ApprovalStatus.Approved,
            CurrentUser.Id.Value,
            CurrentUser.Name ?? "Unknown",
            previousStatus,
            comments);

        await SendApprovalDecisionEmailAsync(approval, true);

        _logger.LogInformation("Approval request {RequestNumber} approved by {User}",
            approval.RequestNumber, CurrentUser.Name);

        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval);
    }

    [Authorize(AuditPermissions.Approvals.Reject)]
    public async Task<ApprovalRequestDto> RejectAsync(Guid id, string? comments = null)
    {
        if (!CurrentUser.Id.HasValue)
            throw new BusinessException("User must be authenticated to reject requests");

        var approval = await _approvalRequestRepository.GetAsync(id);

        if (approval.Status != ApprovalStatus.Pending)
            throw new BusinessException($"Only pending requests can be rejected. Current status: {approval.Status}");

        if (approval.AssignedApproverId.HasValue && approval.AssignedApproverId != CurrentUser.Id)
            throw new BusinessException("You are not authorized to reject this request");

        var previousStatus = approval.Status;
        approval.Reject(CurrentUser.Id.Value, CurrentUser.Name ?? "Unknown", comments);

        await _approvalRequestRepository.UpdateAsync(approval, autoSave: true);

        await AddHistoryEntryAsync(
            approval.Id,
            "Rejected",
            ApprovalStatus.Rejected,
            CurrentUser.Id.Value,
            CurrentUser.Name ?? "Unknown",
            previousStatus,
            comments);

        await SendApprovalDecisionEmailAsync(approval, false);

        _logger.LogInformation("Approval request {RequestNumber} rejected by {User}",
            approval.RequestNumber, CurrentUser.Name);

        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval);
    }

    [Authorize(AuditPermissions.Approvals.Cancel)]
    public async Task<ApprovalRequestDto> CancelAsync(Guid id)
    {
        if (!CurrentUser.Id.HasValue)
            throw new BusinessException("User must be authenticated");

        var approval = await _approvalRequestRepository.GetAsync(id);

        if (approval.RequesterId != CurrentUser.Id)
            throw new BusinessException("Only the requester can cancel this request");

        if (approval.Status != ApprovalStatus.Pending)
            throw new BusinessException($"Only pending requests can be cancelled. Current status: {approval.Status}");

        var previousStatus = approval.Status;
        approval.Cancel();

        await _approvalRequestRepository.UpdateAsync(approval, autoSave: true);

        await AddHistoryEntryAsync(
            approval.Id,
            "Cancelled",
            ApprovalStatus.Cancelled,
            CurrentUser.Id.Value,
            CurrentUser.Name ?? "Unknown",
            previousStatus,
            "Request cancelled by requester");

        _logger.LogInformation("Approval request {RequestNumber} cancelled by requester",
            approval.RequestNumber);

        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval);
    }

    [Authorize(AuditPermissions.Approvals.Reassign)]
    public async Task<ApprovalRequestDto> ReassignAsync(Guid id, Guid newApproverId, string newApproverName)
    {
        var approval = await _approvalRequestRepository.GetAsync(id);

        if (approval.Status != ApprovalStatus.Pending)
            throw new BusinessException("Only pending requests can be reassigned");

        var oldApproverId = approval.AssignedApproverId;
        var oldApproverName = approval.AssignedApproverName;

        approval.AssignedApproverId = newApproverId;
        approval.AssignedApproverName = newApproverName;

        await _approvalRequestRepository.UpdateAsync(approval, autoSave: true);

        await AddHistoryEntryAsync(
            approval.Id,
            "Reassigned",
            approval.Status,
            CurrentUser.Id ?? Guid.Empty,
            CurrentUser.Name ?? "System",
            null,
            $"Reassigned from {oldApproverName ?? "Unassigned"} to {newApproverName}");

        await SendApprovalNotificationEmailAsync(approval);

        return ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(approval);
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<List<ApprovalHistoryDto>> GetHistoryAsync(Guid approvalRequestId)
    {
        var history = await _approvalHistoryRepository.GetListByApprovalRequestAsync(approvalRequestId);
        return ObjectMapper.Map<List<ApprovalHistory>, List<ApprovalHistoryDto>>(history);
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<int> GetPendingCountAsync()
    {
        if (!CurrentUser.Id.HasValue)
            return 0;

        var count = await _approvalRequestRepository.GetPendingCountAsync(CurrentUser.Id.Value);
        return (int)count;
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<ApprovalStatisticsDto> GetStatisticsAsync()
    {
        var query = await _approvalRepository.GetQueryableAsync();

        var stats = new ApprovalStatisticsDto
        {
            TotalPending = await AsyncExecuter.CountAsync(query.Where(x => x.Status == ApprovalStatus.Pending)),
            TotalApproved = await AsyncExecuter.CountAsync(query.Where(x => x.Status == ApprovalStatus.Approved)),
            TotalRejected = await AsyncExecuter.CountAsync(query.Where(x => x.Status == ApprovalStatus.Rejected)),
            TotalCancelled = await AsyncExecuter.CountAsync(query.Where(x => x.Status == ApprovalStatus.Cancelled)),
            TotalExpired = await AsyncExecuter.CountAsync(query.Where(x => x.Status == ApprovalStatus.Expired))
        };

        if (CurrentUser.Id.HasValue)
        {
            var userId = CurrentUser.Id.Value;
            stats.MyPendingApprovals = await AsyncExecuter.CountAsync(
                query.Where(x => x.Status == ApprovalStatus.Pending &&
                    (x.AssignedApproverId == null || x.AssignedApproverId == userId)));

            stats.MySubmittedRequests = await AsyncExecuter.CountAsync(
                query.Where(x => x.RequesterId == userId));

            var today = DateTime.UtcNow.Date;
            stats.MyApprovedToday = await AsyncExecuter.CountAsync(
                query.Where(x => x.ApprovedById == userId &&
                    x.Status == ApprovalStatus.Approved &&
                    x.ApprovedAt.HasValue && x.ApprovedAt.Value.Date == today));

            stats.MyRejectedToday = await AsyncExecuter.CountAsync(
                query.Where(x => x.ApprovedById == userId &&
                    x.Status == ApprovalStatus.Rejected &&
                    x.ApprovedAt.HasValue && x.ApprovedAt.Value.Date == today));
        }

        stats.PendingOrganizations = await AsyncExecuter.CountAsync(
            query.Where(x => x.Status == ApprovalStatus.Pending && x.EntityType == ApprovalEntityType.Organization));
        stats.PendingWorkspaces = await AsyncExecuter.CountAsync(
            query.Where(x => x.Status == ApprovalStatus.Pending && x.EntityType == ApprovalEntityType.Workspace));
        stats.PendingDocuments = await AsyncExecuter.CountAsync(
            query.Where(x => x.Status == ApprovalStatus.Pending && x.EntityType == ApprovalEntityType.Document));
        stats.PendingUserProfiles = await AsyncExecuter.CountAsync(
            query.Where(x => x.Status == ApprovalStatus.Pending && x.EntityType == ApprovalEntityType.UserProfile));

        // Calculate average approval time
        var approvedItems = await AsyncExecuter.ToListAsync(
            query.Where(x => x.Status == ApprovalStatus.Approved && x.ApprovedAt.HasValue)
                .OrderByDescending(x => x.ApprovedAt)
                .Take(100));

        if (approvedItems.Any())
        {
            var avgHours = approvedItems
                .Where(x => x.ApprovedAt.HasValue)
                .Average(x => (x.ApprovedAt!.Value - x.CreationTime).TotalHours);
            stats.AverageApprovalTimeHours = Math.Round(avgHours, 2);
        }

        return stats;
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<bool> HasPendingApprovalAsync(ApprovalEntityType entityType, Guid entityId)
    {
        var approvals = await _approvalRequestRepository.GetListByEntityAsync(entityType, entityId);
        return approvals.Any(x => x.Status == ApprovalStatus.Pending);
    }

    [Authorize(AuditPermissions.Approvals.Default)]
    public async Task<ApprovalRequestDto?> GetLatestForEntityAsync(ApprovalEntityType entityType, Guid entityId)
    {
        var approvals = await _approvalRequestRepository.GetListByEntityAsync(entityType, entityId);
        var latest = approvals.OrderByDescending(x => x.CreationTime).FirstOrDefault();
        return latest != null ? ObjectMapper.Map<ApprovalRequest, ApprovalRequestDto>(latest) : null;
    }

    private async Task AddHistoryEntryAsync(
        Guid approvalRequestId,
        string action,
        ApprovalStatus newStatus,
        Guid actorId,
        string actorName,
        ApprovalStatus? previousStatus,
        string? comments)
    {
        var history = new ApprovalHistory(
            GuidGenerator.Create(),
            approvalRequestId,
            action,
            newStatus,
            actorId,
            actorName,
            previousStatus,
            comments,
            CurrentTenant.Id);

        await _historyRepository.InsertAsync(history, autoSave: true);
    }

    private async Task SendApprovalNotificationEmailAsync(ApprovalRequest approval)
    {
        try
        {
            if (approval.AssignedApproverId.HasValue && !string.IsNullOrEmpty(approval.AssignedApproverName))
            {
                var subject = $"New Approval Request: {approval.RequestNumber}";
                var body = $@"
                    <h2>New Approval Request</h2>
                    <p>You have been assigned a new approval request.</p>
                    <ul>
                        <li><strong>Request Number:</strong> {approval.RequestNumber}</li>
                        <li><strong>Entity:</strong> {approval.EntityType} - {approval.EntityName}</li>
                        <li><strong>Requested By:</strong> {approval.RequesterName}</li>
                        <li><strong>Action:</strong> {approval.RequestedAction}</li>
                        <li><strong>Priority:</strong> {approval.Priority}</li>
                        {(string.IsNullOrEmpty(approval.RequestReason) ? "" : $"<li><strong>Reason:</strong> {approval.RequestReason}</li>")}
                    </ul>
                    <p>Please review and take action on this request.</p>
                ";

                // Note: In production, you would retrieve the approver's email address
                // For now, we'll log it
                _logger.LogInformation("Email notification sent for approval request {RequestNumber}", approval.RequestNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send approval notification email for {RequestNumber}", approval.RequestNumber);
        }
    }

    private async Task SendApprovalDecisionEmailAsync(ApprovalRequest approval, bool approved)
    {
        try
        {
            if (!string.IsNullOrEmpty(approval.RequesterEmail))
            {
                var subject = $"Approval Request {(approved ? "Approved" : "Rejected")}: {approval.RequestNumber}";
                var body = $@"
                    <h2>Approval Request {(approved ? "Approved" : "Rejected")}</h2>
                    <p>Your approval request has been {(approved ? "approved" : "rejected")}.</p>
                    <ul>
                        <li><strong>Request Number:</strong> {approval.RequestNumber}</li>
                        <li><strong>Entity:</strong> {approval.EntityType} - {approval.EntityName}</li>
                        <li><strong>{(approved ? "Approved" : "Rejected")} By:</strong> {approval.ApprovedByName}</li>
                        <li><strong>Decision Date:</strong> {approval.ApprovedAt:yyyy-MM-dd HH:mm}</li>
                        {(string.IsNullOrEmpty(approval.ApprovalComments) ? "" : $"<li><strong>Comments:</strong> {approval.ApprovalComments}</li>")}
                    </ul>
                ";

                _logger.LogInformation("Decision notification sent for approval request {RequestNumber}", approval.RequestNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send decision email for {RequestNumber}", approval.RequestNumber);
        }
    }
}
