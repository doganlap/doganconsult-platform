using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using DoganConsult.Workspace.Workspaces;
using DoganConsult.Web.Blazor.Services;
using Volo.Abp.Application.Dtos;
using Microsoft.JSInterop;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class Workspaces : ComponentBase
{
    [Inject] private WorkspaceService WorkspaceService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<WorkspaceDto> WorkspaceList = new();
    private IEnumerable<WorkspaceDto> WorkspaceItems => WorkspaceList;
    
    private bool Loading = true;
    private bool ShowModal = false;
    private bool ShowDetailsModal = false;
    private bool IsEditing = false;
    private bool Saving = false;
    private string FormErrorMessage = string.Empty;

    private CreateUpdateWorkspaceDto CurrentWorkspace = new();
    private WorkspaceDto? SelectedWorkspace = null;
    private Guid? EditingWorkspaceId = null;

    // Pagination
    private int CurrentPage = 1;
    private int PageSize = 10;
    private int TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected override async Task OnInitializedAsync()
    {
        await LoadWorkspaces();
    }

    private async Task LoadWorkspaces()
    {
        Loading = true;
        try
        {
            var input = new PagedAndSortedResultRequestDto
            {
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize,
                Sorting = "Name"
            };

            var result = await WorkspaceService.GetListAsync(input);
            WorkspaceList = result.Items?.ToList() ?? new List<WorkspaceDto>();
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to load workspaces: {ex.Message}", "error");
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    private void OpenCreateModal()
    {
        IsEditing = false;
        CurrentWorkspace = new CreateUpdateWorkspaceDto
        {
            Status = "active"
        };
        EditingWorkspaceId = null;
        FormErrorMessage = string.Empty;
        ShowModal = true;
    }

    private void EditWorkspace(WorkspaceDto workspace)
    {
        IsEditing = true;
        EditingWorkspaceId = workspace.Id;
        CurrentWorkspace = new CreateUpdateWorkspaceDto
        {
            Code = workspace.Code,
            Name = workspace.Name,
            Description = workspace.Description,
            Status = workspace.Status,
            WorkspaceOwner = workspace.WorkspaceOwner,
            Members = workspace.Members,
            Permissions = workspace.Permissions,
            OrganizationId = workspace.OrganizationId
        };
        FormErrorMessage = string.Empty;
        ShowDetailsModal = false;
        ShowModal = true;
    }

    private async Task SaveWorkspace()
    {
        Saving = true;
        FormErrorMessage = string.Empty;
        try
        {
            if (IsEditing && EditingWorkspaceId.HasValue)
            {
                await WorkspaceService.UpdateAsync(EditingWorkspaceId.Value, CurrentWorkspace);
                await ShowAlert("Success", "Workspace updated successfully!", "success");
            }
            else
            {
                await WorkspaceService.CreateAsync(CurrentWorkspace);
                await ShowAlert("Success", "Workspace created successfully!", "success");
            }
            
            CloseModal();
            await LoadWorkspaces();
        }
        catch (Exception ex)
        {
            FormErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            Saving = false;
        }
    }

    private async Task DeleteWorkspace(WorkspaceDto workspace)
    {
        var confirmed = await ShowConfirm("Confirm Delete", $"Are you sure you want to delete '{workspace.Name}'?");
        if (confirmed)
        {
            try
            {
                await WorkspaceService.DeleteAsync(workspace.Id);
                await ShowAlert("Success", "Workspace deleted successfully!", "success");
                await LoadWorkspaces();
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"Failed to delete: {ex.Message}", "error");
            }
        }
    }

    private void ViewDetails(WorkspaceDto workspace)
    {
        SelectedWorkspace = workspace;
        ShowDetailsModal = true;
    }

    private void CloseModal()
    {
        ShowModal = false;
        CurrentWorkspace = new CreateUpdateWorkspaceDto();
        EditingWorkspaceId = null;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedWorkspace = null;
    }

    private async Task GoToPage(int page)
    {
        CurrentPage = page;
        await LoadWorkspaces();
    }

    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadWorkspaces();
        }
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadWorkspaces();
        }
    }

    private static string GetStatusColor(string? status) => status?.ToLower() switch
    {
        "active" => "success",
        "inactive" => "secondary",
        "archived" => "warning",
        _ => "info"
    };

    private async Task ShowAlert(string title, string message, string type)
    {
        await JSRuntime.InvokeVoidAsync("alert", $"{title}: {message}");
    }

    private async Task<bool> ShowConfirm(string title, string message)
    {
        return await JSRuntime.InvokeAsync<bool>("confirm", message);
    }
}
