using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Web.Blazor.Services;
using Volo.Abp.Application.Dtos;
using Microsoft.JSInterop;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class AuditLogs : ComponentBase
{
    [Inject] private AuditService AuditService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<AuditLogDto> AuditLogList = new();
    private IEnumerable<AuditLogDto> AuditLogItems => AuditLogList;
    
    private bool Loading = true;
    private bool ShowDetailsModal = false;
    private AuditLogDto? SelectedLog = null;

    // Filters
    private string FilterAction = string.Empty;
    private string FilterEntity = string.Empty;
    private string FilterStatus = string.Empty;
    private string SearchUser = string.Empty;

    // Pagination
    private int CurrentPage = 1;
    private int PageSize = 20;
    private int TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected override async Task OnInitializedAsync()
    {
        await LoadLogs();
    }

    private async Task LoadLogs()
    {
        Loading = true;
        try
        {
            var input = new PagedAndSortedResultRequestDto
            {
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize,
                Sorting = "CreationTime desc"
            };

            var result = await AuditService.GetListAsync(input);
            var logs = result.Items?.ToList() ?? new List<AuditLogDto>();
            
            // Apply client-side filters (ideally these would be server-side)
            if (!string.IsNullOrEmpty(FilterAction))
            {
                logs = logs.Where(l => l.Action.Equals(FilterAction, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(FilterEntity))
            {
                logs = logs.Where(l => l.EntityType.Contains(FilterEntity, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            if (!string.IsNullOrEmpty(FilterStatus))
            {
                logs = logs.Where(l => l.Status?.Equals(FilterStatus, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }
            if (!string.IsNullOrEmpty(SearchUser))
            {
                logs = logs.Where(l => l.UserName?.Contains(SearchUser, StringComparison.OrdinalIgnoreCase) == true).ToList();
            }
            
            AuditLogList = logs;
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to load audit logs: {ex.Message}");
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    private async Task RefreshLogs()
    {
        await LoadLogs();
    }

    private void ViewDetails(AuditLogDto log)
    {
        SelectedLog = log;
        ShowDetailsModal = true;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedLog = null;
    }

    private async Task GoToPage(int page)
    {
        CurrentPage = page;
        await LoadLogs();
    }

    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadLogs();
        }
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadLogs();
        }
    }

    private static string GetActionColor(string? action) => action?.ToLower() switch
    {
        "create" => "success",
        "update" => "warning",
        "delete" => "danger",
        "read" => "info",
        "login" => "primary",
        "logout" => "secondary",
        _ => "dark"
    };

    private static string GetActionIcon(string? action) => action?.ToLower() switch
    {
        "create" => "fas fa-plus",
        "update" => "fas fa-edit",
        "delete" => "fas fa-trash",
        "read" => "fas fa-eye",
        "login" => "fas fa-sign-in-alt",
        "logout" => "fas fa-sign-out-alt",
        _ => "fas fa-circle"
    };

    private static string GetStatusColor(string? status) => status?.ToLower() switch
    {
        "success" => "success",
        "failure" or "failed" => "danger",
        "warning" => "warning",
        _ => "secondary"
    };

    private async Task ShowAlert(string title, string message)
    {
        await JSRuntime.InvokeVoidAsync("alert", $"{title}: {message}");
    }
}
