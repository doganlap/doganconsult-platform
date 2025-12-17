using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Services;

public class ApprovalService : ITransientDependency
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApprovalService> _logger;
    private const string BaseUrl = "https://localhost:44375/api/audit/approvals";

    public ApprovalService(HttpClient httpClient, ILogger<ApprovalService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<PagedResultDto<ApprovalRequestDto>> GetListAsync(GetApprovalRequestListDto input)
    {
        try
        {
            var url = $"{BaseUrl}?SkipCount={input.SkipCount}&MaxResultCount={input.MaxResultCount}";
            
            if (input.Status.HasValue)
                url += $"&Status={input.Status.Value}";
            if (input.EntityType.HasValue)
                url += $"&EntityType={input.EntityType.Value}";
            if (input.PendingOnly == true)
                url += "&PendingOnly=true";
            if (input.MyRequestsOnly == true)
                url += "&MyRequestsOnly=true";
            if (input.AssignedToMe == true)
                url += "&AssignedToMe=true";
            
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<ApprovalRequestDto>>(url);
            return response ?? new PagedResultDto<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching approval requests");
            return new PagedResultDto<ApprovalRequestDto>();
        }
    }

    public async Task<PagedResultDto<ApprovalRequestDto>> GetMyPendingApprovalsAsync(int skipCount = 0, int maxResultCount = 10)
    {
        try
        {
            var url = $"{BaseUrl}/my-pending?SkipCount={skipCount}&MaxResultCount={maxResultCount}";
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<ApprovalRequestDto>>(url);
            return response ?? new PagedResultDto<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching my pending approvals");
            return new PagedResultDto<ApprovalRequestDto>();
        }
    }

    public async Task<PagedResultDto<ApprovalRequestDto>> GetMyRequestsAsync(int skipCount = 0, int maxResultCount = 10)
    {
        try
        {
            var url = $"{BaseUrl}/my-requests?SkipCount={skipCount}&MaxResultCount={maxResultCount}";
            var response = await _httpClient.GetFromJsonAsync<PagedResultDto<ApprovalRequestDto>>(url);
            return response ?? new PagedResultDto<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching my requests");
            return new PagedResultDto<ApprovalRequestDto>();
        }
    }

    public async Task<ApprovalRequestDto?> GetAsync(Guid id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApprovalRequestDto>($"{BaseUrl}/{id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching approval request {Id}", id);
            return null;
        }
    }

    public async Task<ApprovalRequestDto?> ApproveAsync(Guid id, string? comments = null)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/approve", new { Comments = comments });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving request {Id}", id);
            throw;
        }
    }

    public async Task<ApprovalRequestDto?> RejectAsync(Guid id, string? comments = null)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/reject", new { Comments = comments });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rejecting request {Id}", id);
            throw;
        }
    }

    public async Task<ApprovalRequestDto?> CancelAsync(Guid id)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{id}/cancel", new { });
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApprovalRequestDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling request {Id}", id);
            throw;
        }
    }

    public async Task<List<ApprovalHistoryDto>> GetHistoryAsync(Guid approvalRequestId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ApprovalHistoryDto>>($"{BaseUrl}/{approvalRequestId}/history");
            return response ?? new List<ApprovalHistoryDto>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching history for {Id}", approvalRequestId);
            return new List<ApprovalHistoryDto>();
        }
    }

    public async Task<ApprovalStatisticsDto?> GetStatisticsAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ApprovalStatisticsDto>($"{BaseUrl}/statistics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching approval statistics");
            return null;
        }
    }

    public async Task<int> GetPendingCountAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<int>($"{BaseUrl}/pending-count");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching pending count");
            return 0;
        }
    }
}

// DTOs for client-side
public class ApprovalRequestDto
{
    public Guid Id { get; set; }
    public string RequestNumber { get; set; } = string.Empty;
    public ApprovalEntityType EntityType { get; set; }
    public Guid EntityId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public ApprovalStatus Status { get; set; }
    public ApprovalPriority Priority { get; set; }
    public Guid RequesterId { get; set; }
    public string RequesterName { get; set; } = string.Empty;
    public string RequestedAction { get; set; } = string.Empty;
    public string? RequestReason { get; set; }
    public Guid? AssignedApproverId { get; set; }
    public string? AssignedApproverName { get; set; }
    public Guid? ApprovedById { get; set; }
    public string? ApprovedByName { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalComments { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreationTime { get; set; }
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public string StatusDisplay => Status switch
    {
        ApprovalStatus.Pending => "Pending",
        ApprovalStatus.Approved => "Approved",
        ApprovalStatus.Rejected => "Rejected",
        ApprovalStatus.Cancelled => "Cancelled",
        ApprovalStatus.Expired => "Expired",
        _ => Status.ToString()
    };
}

public class ApprovalHistoryDto
{
    public Guid Id { get; set; }
    public Guid ApprovalRequestId { get; set; }
    public string Action { get; set; } = string.Empty;
    public ApprovalStatus? PreviousStatus { get; set; }
    public ApprovalStatus NewStatus { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; } = string.Empty;
    public string? Comments { get; set; }
    public DateTime CreationTime { get; set; }
}

public class ApprovalStatisticsDto
{
    public int TotalPending { get; set; }
    public int TotalApproved { get; set; }
    public int TotalRejected { get; set; }
    public int TotalCancelled { get; set; }
    public int TotalExpired { get; set; }
    public int MyPendingApprovals { get; set; }
    public int MySubmittedRequests { get; set; }
    public int MyApprovedToday { get; set; }
    public int MyRejectedToday { get; set; }
    public int PendingOrganizations { get; set; }
    public int PendingWorkspaces { get; set; }
    public int PendingDocuments { get; set; }
    public int PendingUserProfiles { get; set; }
    public double AverageApprovalTimeHours { get; set; }
}

public class GetApprovalRequestListDto
{
    public int SkipCount { get; set; }
    public int MaxResultCount { get; set; } = 10;
    public ApprovalStatus? Status { get; set; }
    public ApprovalEntityType? EntityType { get; set; }
    public bool? PendingOnly { get; set; }
    public bool? MyRequestsOnly { get; set; }
    public bool? AssignedToMe { get; set; }
}

public enum ApprovalStatus
{
    Pending = 0,
    Approved = 1,
    Rejected = 2,
    Cancelled = 3,
    Expired = 4
}

public enum ApprovalEntityType
{
    Organization = 0,
    Workspace = 1,
    Document = 2,
    UserProfile = 3,
    AIConfiguration = 4,
    Custom = 99
}

public enum ApprovalPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Urgent = 3
}
