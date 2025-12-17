using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DoganConsult.Web.Blazor.Services;
using Microsoft.Extensions.Logging;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class Approvals
{
    [Inject]
    public ApprovalService ApprovalService { get; set; } = default!;

    [Inject]
    public ILogger<Approvals> Logger { get; set; } = default!;

    // State
    private bool Loading { get; set; } = true;
    private bool Processing { get; set; }
    private string ActiveTab { get; set; } = "pending";
    private List<ApprovalRequestDto>? ApprovalItems { get; set; }
    private ApprovalStatisticsDto? Statistics { get; set; }
    private List<ApprovalHistoryDto>? ApprovalHistory { get; set; }

    // Pagination
    private int CurrentPage { get; set; } = 1;
    private int PageSize { get; set; } = 10;
    private long TotalCount { get; set; }
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Modal State
    private bool ShowDetailsModal { get; set; }
    private bool ShowActionModal { get; set; }
    private bool IsApproving { get; set; }
    private ApprovalRequestDto? SelectedApproval { get; set; }
    private string ActionComments { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadStatistics();
        await LoadData();
    }

    private async Task LoadStatistics()
    {
        try
        {
            Statistics = await ApprovalService.GetStatisticsAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading statistics");
        }
    }

    private async Task LoadData()
    {
        Loading = true;
        StateHasChanged();

        try
        {
            var skipCount = (CurrentPage - 1) * PageSize;

            var result = ActiveTab switch
            {
                "pending" => await ApprovalService.GetMyPendingApprovalsAsync(skipCount, PageSize),
                "myrequests" => await ApprovalService.GetMyRequestsAsync(skipCount, PageSize),
                _ => await ApprovalService.GetListAsync(new GetApprovalRequestListDto
                {
                    SkipCount = skipCount,
                    MaxResultCount = PageSize
                })
            };

            ApprovalItems = result.Items.ToList();
            TotalCount = result.TotalCount;
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error loading approval requests");
            ApprovalItems = new List<ApprovalRequestDto>();
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    private async Task RefreshData()
    {
        await LoadStatistics();
        await LoadData();
    }

    private async Task SwitchTab(string tab)
    {
        ActiveTab = tab;
        CurrentPage = 1;
        await LoadData();
    }

    private string GetTabTitle()
    {
        return ActiveTab switch
        {
            "pending" => "My Pending Approvals",
            "myrequests" => "My Submitted Requests",
            _ => "All Approval Requests"
        };
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadData();
        }
    }

    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadData();
        }
    }

    private async Task ViewDetails(ApprovalRequestDto approval)
    {
        SelectedApproval = approval;
        ApprovalHistory = await ApprovalService.GetHistoryAsync(approval.Id);
        ShowDetailsModal = true;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedApproval = null;
        ApprovalHistory = null;
    }

    private void OpenApproveModal(ApprovalRequestDto approval)
    {
        SelectedApproval = approval;
        IsApproving = true;
        ActionComments = string.Empty;
        ShowActionModal = true;
    }

    private void OpenRejectModal(ApprovalRequestDto approval)
    {
        SelectedApproval = approval;
        IsApproving = false;
        ActionComments = string.Empty;
        ShowActionModal = true;
    }

    private void CloseActionModal()
    {
        ShowActionModal = false;
        SelectedApproval = null;
        ActionComments = string.Empty;
    }

    private async Task ProcessAction()
    {
        if (SelectedApproval == null) return;

        Processing = true;
        StateHasChanged();

        try
        {
            if (IsApproving)
            {
                await ApprovalService.ApproveAsync(SelectedApproval.Id, ActionComments);
            }
            else
            {
                await ApprovalService.RejectAsync(SelectedApproval.Id, ActionComments);
            }

            CloseActionModal();
            await RefreshData();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Error processing approval action");
        }
        finally
        {
            Processing = false;
            StateHasChanged();
        }
    }

    // Helper methods for styling
    private static string GetStatusColor(ApprovalStatus status) => status switch
    {
        ApprovalStatus.Pending => "warning",
        ApprovalStatus.Approved => "success",
        ApprovalStatus.Rejected => "danger",
        ApprovalStatus.Cancelled => "secondary",
        ApprovalStatus.Expired => "dark",
        _ => "secondary"
    };

    private static string GetPriorityColor(ApprovalPriority priority) => priority switch
    {
        ApprovalPriority.Low => "secondary",
        ApprovalPriority.Normal => "info",
        ApprovalPriority.High => "warning",
        ApprovalPriority.Urgent => "danger",
        _ => "secondary"
    };

    private static string GetEntityTypeColor(ApprovalEntityType entityType) => entityType switch
    {
        ApprovalEntityType.Organization => "primary",
        ApprovalEntityType.Workspace => "info",
        ApprovalEntityType.Document => "success",
        ApprovalEntityType.UserProfile => "warning",
        ApprovalEntityType.AIConfiguration => "purple",
        _ => "secondary"
    };
}
