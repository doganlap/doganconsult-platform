using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using DoganConsult.UserProfile.UserProfiles;
using DoganConsult.Web.Blazor.Services;
using Volo.Abp.Application.Dtos;
using Microsoft.JSInterop;

namespace DoganConsult.Web.Blazor.Components.Pages;

public partial class UserProfiles : ComponentBase
{
    [Inject] private UserProfileService UserProfileService { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    private List<UserProfileDto> UserProfileList = new();
    private IEnumerable<UserProfileDto> UserProfileItems => UserProfileList;
    
    private bool Loading = true;
    private bool ShowModal = false;
    private bool ShowDetailsModal = false;
    private bool IsEditing = false;
    private bool Saving = false;
    private string FormErrorMessage = string.Empty;

    private CreateUpdateUserProfileDto CurrentUser = new();
    private UserProfileDto? SelectedUser = null;
    private Guid? EditingUserId = null;

    // Pagination
    private int CurrentPage = 1;
    private int PageSize = 10;
    private int TotalCount = 0;
    private int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

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

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        Loading = true;
        try
        {
            var input = new PagedAndSortedResultRequestDto
            {
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize,
                Sorting = "FullName"
            };

            var result = await UserProfileService.GetListAsync(input);
            UserProfileList = result.Items?.ToList() ?? new List<UserProfileDto>();
            TotalCount = (int)result.TotalCount;
        }
        catch (Exception ex)
        {
            await ShowAlert("Error", $"Failed to load users: {ex.Message}", "error");
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
        CurrentUser = new CreateUpdateUserProfileDto
        {
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            ExternalUserId = Guid.NewGuid().ToString()
        };
        EditingUserId = null;
        FormErrorMessage = string.Empty;
        ShowModal = true;
    }

    private void EditUser(UserProfileDto user)
    {
        IsEditing = true;
        EditingUserId = user.Id;
        CurrentUser = new CreateUpdateUserProfileDto
        {
            ExternalUserId = user.ExternalUserId,
            Email = user.Email,
            FullName = user.FullName,
            SystemRole = user.SystemRole,
            StakeholderType = user.StakeholderType,
            PrimaryRole = user.PrimaryRole,
            OrganizationId = user.OrganizationId,
            AssignedClients = user.AssignedClients,
            JobTitle = user.JobTitle,
            Phone = user.Phone,
            Country = user.Country,
            Location = user.Location,
            Department = user.Department,
            Bio = user.Bio,
            Skills = user.Skills,
            StartDate = user.StartDate
        };
        FormErrorMessage = string.Empty;
        ShowDetailsModal = false;
        ShowModal = true;
    }

    private async Task SaveUser()
    {
        Saving = true;
        FormErrorMessage = string.Empty;
        try
        {
            if (IsEditing && EditingUserId.HasValue)
            {
                await UserProfileService.UpdateAsync(EditingUserId.Value, CurrentUser);
                await ShowAlert("Success", "User profile updated successfully!", "success");
            }
            else
            {
                await UserProfileService.CreateAsync(CurrentUser);
                await ShowAlert("Success", "User profile created successfully!", "success");
            }
            
            CloseModal();
            await LoadUsers();
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

    private async Task DeleteUser(UserProfileDto user)
    {
        var confirmed = await ShowConfirm("Confirm Delete", $"Are you sure you want to delete '{user.FullName}'?");
        if (confirmed)
        {
            try
            {
                await UserProfileService.DeleteAsync(user.Id);
                await ShowAlert("Success", "User profile deleted successfully!", "success");
                await LoadUsers();
            }
            catch (Exception ex)
            {
                await ShowAlert("Error", $"Failed to delete: {ex.Message}", "error");
            }
        }
    }

    private void ViewDetails(UserProfileDto user)
    {
        SelectedUser = user;
        ShowDetailsModal = true;
    }

    private void CloseModal()
    {
        ShowModal = false;
        CurrentUser = new CreateUpdateUserProfileDto();
        EditingUserId = null;
    }

    private void CloseDetailsModal()
    {
        ShowDetailsModal = false;
        SelectedUser = null;
    }

    private async Task GoToPage(int page)
    {
        CurrentPage = page;
        await LoadUsers();
    }

    private async Task PreviousPage()
    {
        if (CurrentPage > 1)
        {
            CurrentPage--;
            await LoadUsers();
        }
    }

    private async Task NextPage()
    {
        if (CurrentPage < TotalPages)
        {
            CurrentPage++;
            await LoadUsers();
        }
    }

    private static string GetRoleColor(SystemRoleDto role) => role switch
    {
        SystemRoleDto.Admin => "danger",
        SystemRoleDto.User => "primary",
        _ => "secondary"
    };

    private static string GetStakeholderLabel(StakeholderTypeDto type) => type switch
    {
        StakeholderTypeDto.SuperAdmin => "Super Admin",
        StakeholderTypeDto.ConsultantDelivery => "Consultant",
        StakeholderTypeDto.ClientOwner => "Client Owner",
        StakeholderTypeDto.ClientUserBusiness => "Business User",
        StakeholderTypeDto.RegulatorAuditor => "Regulator/Auditor",
        StakeholderTypeDto.PartnerVendor => "Partner/Vendor",
        StakeholderTypeDto.DemoGuest => "Demo/Guest",
        _ => type.ToString()
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
