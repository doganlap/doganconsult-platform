using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using DoganConsult.Web.Blazor.Organizations;
using DoganConsult.Web.Blazor.Services;
using Volo.Abp.Application.Dtos;
using Microsoft.JSInterop;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class Organizations : ComponentBase
{
    [Inject] private OrganizationService OrganizationService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<OrganizationDto> OrganizationList = new();
    private IEnumerable<OrganizationDto> OrganizationItems => OrganizationList;
    
    private bool Loading = true;
    private bool ShowModal = false;
    private bool ShowDetailsModal = false;
    private bool IsEditing = false;
    private bool Saving = false;
    private string FormErrorMessage = string.Empty;

    private CreateUpdateOrganizationDto CurrentOrganization = new();
    private OrganizationDto? SelectedOrganization = null;
    private Guid? EditingOrganizationId = null;
    private EditContext? editContext;

    // Pagination
    private int CurrentPage = 1;
    private int PageSize = 10;
    private int TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    // Professional dropdown lists
    private readonly List<string> CountryList = new()
    {
        "Afghanistan", "Albania", "Algeria", "Argentina", "Armenia", "Australia",
        "Austria", "Azerbaijan", "Bahrain", "Bangladesh", "Belarus", "Belgium",
        "Brazil", "Bulgaria", "Cambodia", "Canada", "Chile", "China",
        "Colombia", "Croatia", "Czech Republic", "Denmark", "Egypt", "Estonia",
        "Finland", "France", "Georgia", "Germany", "Ghana", "Greece",
        "Hungary", "Iceland", "India", "Indonesia", "Iran", "Iraq",
        "Ireland", "Israel", "Italy", "Japan", "Jordan", "Kazakhstan",
        "Kenya", "Kuwait", "Latvia", "Lebanon", "Lithuania", "Luxembourg",
        "Malaysia", "Mexico", "Morocco", "Netherlands", "New Zealand", "Nigeria",
        "Norway", "Oman", "Pakistan", "Philippines", "Poland", "Portugal",
        "Qatar", "Romania", "Russia", "Saudi Arabia", "Singapore", "Slovakia",
        "Slovenia", "South Africa", "South Korea", "Spain", "Sweden", "Switzerland",
        "Thailand", "Turkey", "UAE", "Ukraine", "United Kingdom", "United States",
        "Vietnam", "Yemen"
    };

    private readonly List<string> SectorList = new()
    {
        "Technology", "Financial Services", "Healthcare", "Manufacturing",
        "Energy", "Telecommunications", "Retail", "Real Estate",
        "Transportation", "Education", "Government", "Non-Profit",
        "Media & Entertainment", "Agriculture", "Construction", "Mining",
        "Aerospace", "Automotive", "Pharmaceutical", "Food & Beverage",
        "Consulting", "Legal Services", "Insurance", "Banking",
        "Oil & Gas", "Renewable Energy", "Logistics", "Tourism",
        "Defense", "Research & Development", "Other"
    };

    protected override async Task OnInitializedAsync()
    {
        await LoadOrganizations();
    }

    private async Task LoadOrganizations()
    {
        Loading = true;
        try
        {
            var input = new PagedAndSortedResultRequestDto
            {
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize,
                Sorting = "OrganizationName"
            };

            var result = await OrganizationService.GetListAsync(input);
            OrganizationList = result.Items?.ToList() ?? new List<OrganizationDto>();
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to load organizations: {ex.Message}", "error");
        }
        finally
        {
            Loading = false;
            StateHasChanged();
        }
    }

    private void OpenCreateModal()
    {
        CurrentOrganization = new CreateUpdateOrganizationDto();
        editContext = new EditContext(CurrentOrganization);
        IsEditing = false;
        EditingOrganizationId = null;
        FormErrorMessage = string.Empty;
        ShowModal = true;
    }

    private void EditOrganization(OrganizationDto org)
    {
        CurrentOrganization = new CreateUpdateOrganizationDto
        {
            OrganizationCode = org.OrganizationCode,
            OrganizationName = org.OrganizationName,
            OrganizationType = org.OrganizationType,
            Country = org.Country,
            City = org.City,
            Sector = org.Sector,
            RegulatoryRequirements = org.RegulatoryRequirements,
            ContactInformation = org.ContactInformation,
            OrganizationStatus = org.OrganizationStatus
        };
        editContext = new EditContext(CurrentOrganization);
        IsEditing = true;
        EditingOrganizationId = org.Id;
        FormErrorMessage = string.Empty;
        ShowModal = true;
    }

    private void ViewDetails(OrganizationDto org)
    {
        SelectedOrganization = org;
        ShowDetailsModal = true;
    }

    private void CloseModal()
    {
        ShowModal = false;
        CurrentOrganization = new CreateUpdateOrganizationDto();
        editContext = null;
        IsEditing = false;
        EditingOrganizationId = null;
        FormErrorMessage = string.Empty;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedOrganization = null;
    }

    private async Task SaveOrganization()
    {
        FormErrorMessage = string.Empty;
        
        // Validate form
        if (editContext == null || !editContext.Validate())
        {
            FormErrorMessage = "Please fix the validation errors before saving.";
            return;
        }

        // Additional business validation
        var validationResult = ValidateOrganization(CurrentOrganization);
        if (!validationResult.IsValid)
        {
            FormErrorMessage = validationResult.ErrorMessage;
            return;
        }

        Saving = true;
        try
        {
            if (IsEditing && EditingOrganizationId.HasValue)
            {
                await OrganizationService.UpdateAsync(EditingOrganizationId.Value, CurrentOrganization);
                await ShowAlert("Success", "Organization updated successfully!", "success");
            }
            else
            {
                await OrganizationService.CreateAsync(CurrentOrganization);
                await ShowAlert("Success", "Organization created successfully!", "success");
            }

            CloseModal();
            await LoadOrganizations();
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to save organization: {ex.Message}", "error");
        }
        finally
        {
            Saving = false;
            StateHasChanged();
        }
    }

    private async Task DeleteOrganization(OrganizationDto org)
    {
        var confirmed = await JSRuntime.InvokeAsync<bool>("confirm", 
            $"Are you sure you want to delete '{org.OrganizationName}'? This action cannot be undone.");
        
        if (!confirmed) return;

        try
        {
            await OrganizationService.DeleteAsync(org.Id);
            await ShowAlert("Success", "Organization deleted successfully!", "success");
            await LoadOrganizations();
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to delete organization: {ex.Message}", "error");
        }
    }

    private async Task GoToPage(int page)
    {
        if (page < 1 || page > TotalPages) return;
        
        CurrentPage = page;
        await LoadOrganizations();
    }

    private string GetTypeDisplayName(OrganizationType type) => type switch
    {
        OrganizationType.Internal => "Internal Organization",
        OrganizationType.Client => "Client Organization",
        OrganizationType.Regulator => "Regulatory Body",
        OrganizationType.Demo => "Demo/Testing",
        OrganizationType.Other => "Other",
        _ => type.ToString()
    };

    private string GetStatusDisplayName(OrganizationStatus status) => status switch
    {
        OrganizationStatus.Active => "Active & Operational",
        OrganizationStatus.Pilot => "Pilot Program",
        OrganizationStatus.Trial => "Trial Period",
        OrganizationStatus.Inactive => "Inactive",
        _ => status.ToString()
    };

    private (bool IsValid, string ErrorMessage) ValidateOrganization(CreateUpdateOrganizationDto org)
    {
        // Check required fields
        if (string.IsNullOrWhiteSpace(org.OrganizationCode))
            return (false, "Organization Code is required.");
        
        if (string.IsNullOrWhiteSpace(org.OrganizationName))
            return (false, "Organization Name is required.");
        
        // Validate code format (alphanumeric, hyphens, underscores)
        if (!System.Text.RegularExpressions.Regex.IsMatch(org.OrganizationCode, @"^[a-zA-Z0-9_-]+$"))
            return (false, "Organization Code can only contain letters, numbers, hyphens, and underscores.");
        
        // Check for duplicate codes (basic validation - server will do final check)
        if (!IsEditing && OrganizationList.Any(o => 
            string.Equals(o.OrganizationCode, org.OrganizationCode, StringComparison.OrdinalIgnoreCase)))
            return (false, "Organization Code already exists. Please choose a different code.");
        
        // Validate name length
        if (org.OrganizationName.Length < 2)
            return (false, "Organization Name must be at least 2 characters long.");
        
        return (true, string.Empty);
    }

    private string GetTypeColor(OrganizationType type) => type switch
    {
        OrganizationType.Internal => "primary",
        OrganizationType.Client => "success",
        OrganizationType.Regulator => "warning",
        OrganizationType.Demo => "info",
        OrganizationType.Other => "secondary",
        _ => "secondary"
    };

    private string GetStatusColor(OrganizationStatus status) => status switch
    {
        OrganizationStatus.Active => "success",
        OrganizationStatus.Pilot => "warning",
        OrganizationStatus.Trial => "info",
        OrganizationStatus.Inactive => "secondary",
        _ => "secondary"
    };

    private async Task ShowAlert(string title, string message, string type)
    {
        var icon = type switch
        {
            "success" => "success",
            "error" => "error",
            "warning" => "warning",
            _ => "info"
        };

        await JSRuntime.InvokeVoidAsync("Swal.fire", new
        {
            title,
            text = message,
            icon,
            timer = 3000,
            showConfirmButton = false,
            toast = true,
            position = "top-end"
        });
    }
}