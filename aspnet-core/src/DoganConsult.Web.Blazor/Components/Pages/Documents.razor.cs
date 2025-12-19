using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Document.Documents;
using DoganConsult.Document.Permissions;
using DoganConsult.Web.Blazor.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Authorization.Permissions;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class Documents : ComponentBase
{
    [Inject] private DocumentService DocumentService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] private IPermissionChecker PermissionChecker { get; set; } = default!;

    private List<DocumentDto> DocumentList = new();
    private IEnumerable<DocumentDto> DocumentItems => DocumentList;
    
    // Permission checks
    private bool CanCreate = false;
    private bool CanEdit = false;
    private bool CanDelete = false;
    private bool CanViewAll = false;
    
    private bool Loading = true;
    private bool ShowModal = false;
    private bool ShowDetailsModal = false;
    private bool IsEditing = false;
    private bool Saving = false;
    private string FormErrorMessage = string.Empty;

    private CreateUpdateDocumentDto CurrentDocument = new();
    private DocumentDto? SelectedDocument = null;
    private Guid? EditingDocumentId = null;

    // Pagination
    private int CurrentPage = 1;
    private int PageSize = 10;
    private int TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    protected override async Task OnInitializedAsync()
    {
        await LoadPermissions();
        await LoadDocuments();
    }

    private async Task LoadPermissions()
    {
        CanCreate = await PermissionChecker.IsGrantedAsync(DocumentPermissions.Documents.Create);
        CanEdit = await PermissionChecker.IsGrantedAsync(DocumentPermissions.Documents.Edit);
        CanDelete = await PermissionChecker.IsGrantedAsync(DocumentPermissions.Documents.Delete);
        CanViewAll = await PermissionChecker.IsGrantedAsync(DocumentPermissions.Documents.ViewAll);
    }

    private async Task LoadDocuments()
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

            var result = await DocumentService.GetListAsync(input);
            DocumentList = result.Items?.ToList() ?? new List<DocumentDto>();
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to load documents: {ex.Message}", "error");
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
        CurrentDocument = new CreateUpdateDocumentDto
        {
            Status = "active",
            Version = 1,
            UploadDate = DateTime.Now
        };
        EditingDocumentId = null;
        FormErrorMessage = string.Empty;
        ShowModal = true;
    }

    private void EditDocument(DocumentDto document)
    {
        IsEditing = true;
        EditingDocumentId = document.Id;
        CurrentDocument = new CreateUpdateDocumentDto
        {
            Name = document.Name,
            Description = document.Description,
            FileName = document.FileName,
            FileType = document.FileType,
            FileSize = document.FileSize,
            FilePath = document.FilePath,
            Category = document.Category,
            Status = document.Status,
            Version = document.Version,
            ParentDocumentId = document.ParentDocumentId,
            OrganizationId = document.OrganizationId,
            WorkspaceId = document.WorkspaceId,
            DocumentCategory = document.DocumentCategory,
            StoragePath = document.StoragePath,
            UploadedBy = document.UploadedBy,
            UploadDate = document.UploadDate,
            Tags = document.Tags
        };
        FormErrorMessage = string.Empty;
        ShowDetailsModal = false;
        ShowModal = true;
    }

    private async Task SaveDocument()
    {
        Saving = true;
        FormErrorMessage = string.Empty;
        try
        {
            if (IsEditing && EditingDocumentId.HasValue)
            {
                await DocumentService.UpdateAsync(EditingDocumentId.Value, CurrentDocument);
                await ShowAlert("Success", "Document updated successfully!", "success");
            }
            else
            {
                await DocumentService.CreateAsync(CurrentDocument);
                await ShowAlert("Success", "Document created successfully!", "success");
            }
            
            CloseModal();
            await LoadDocuments();
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

    private async Task DeleteDocument(DocumentDto document)
    {
        var confirmed = await ShowConfirm("Confirm Delete", $"Are you sure you want to delete '{document.Name}'?");
        if (confirmed)
        {
            try
            {
                await DocumentService.DeleteAsync(document.Id);
                await ShowAlert("Success", "Document deleted successfully!", "success");
                await LoadDocuments();
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"Failed to delete: {ex.Message}", "error");
            }
        }
    }

    private void ViewDetails(DocumentDto document)
    {
        SelectedDocument = document;
        ShowDetailsModal = true;
    }

    private void CloseModal()
    {
        ShowModal = false;
        CurrentDocument = new CreateUpdateDocumentDto();
        EditingDocumentId = null;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedDocument = null;
    }

    private async Task GoToPage(int page)
    {
        CurrentPage = page;
        await LoadDocuments();
    }

    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadDocuments();
        }
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadDocuments();
        }
    }

    private static string GetStatusColor(string? status) => status?.ToLower() switch
    {
        "active" => "success",
        "draft" => "secondary",
        "archived" => "warning",
        "pending" => "info",
        _ => "info"
    };

    private static string GetFileIcon(string? fileType) => fileType?.ToLower() switch
    {
        "pdf" => "fas fa-file-pdf text-danger",
        "docx" or "doc" => "fas fa-file-word text-primary",
        "xlsx" or "xls" => "fas fa-file-excel text-success",
        "pptx" or "ppt" => "fas fa-file-powerpoint text-warning",
        "txt" => "fas fa-file-alt",
        "csv" => "fas fa-file-csv text-success",
        "json" or "xml" => "fas fa-file-code text-info",
        _ => "fas fa-file"
    };

    private static string FormatFileSize(long? bytes)
    {
        if (bytes == null || bytes == 0) return "-";
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes.Value;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }

    private async Task ShowAlert(string title, string message, string type)
    {
        await JSRuntime.InvokeVoidAsync("alert", $"{title}: {message}");
    }

    private async Task<bool> ShowConfirm(string title, string message)
    {
        return await JSRuntime.InvokeAsync<bool>("confirm", message);
    }
}
