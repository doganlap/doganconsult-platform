using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.UserProfile.UserProfiles;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.UserProfile.UserProfiles;

[Authorize]
public class UserProfileAppService : ApplicationService, IUserProfileAppService
{
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;

    public UserProfileAppService(IRepository<UserProfile, Guid> userProfileRepository)
    {
        _userProfileRepository = userProfileRepository;
    }

    public async Task<UserProfileDto> CreateAsync(CreateUpdateUserProfileDto input)
    {
        var userProfile = new UserProfile(
            GuidGenerator.Create(),
            input.ExternalUserId,
            input.Email,
            input.FullName,
            input.OrganizationId,
            CurrentTenant.Id
        )
        {
            SystemRole = (SystemRole)input.SystemRole,
            StakeholderType = (StakeholderType)input.StakeholderType,
            PrimaryRole = input.PrimaryRole,
            AssignedClients = input.AssignedClients,
            JobTitle = input.JobTitle,
            Phone = input.Phone,
            Country = input.Country,
            Location = input.Location,
            Department = input.Department,
            Bio = input.Bio,
            Skills = input.Skills,
            Title = input.Title,
            AvatarUrl = input.AvatarUrl,
            ManagerId = input.ManagerId,
            StartDate = input.StartDate,
            Availability = input.Availability ?? "Available",
            ProfileCompleted = input.ProfileCompleted
        };

        await _userProfileRepository.InsertAsync(userProfile);
        return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
    }

    public async Task<UserProfileDto> GetAsync(Guid id)
    {
        var userProfile = await _userProfileRepository.GetAsync(id);
        return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
    }

    public async Task<PagedResultDto<UserProfileDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _userProfileRepository.GetQueryableAsync();
        var userProfiles = queryable
            .OrderBy(x => x.FullName)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        var totalCount = await _userProfileRepository.GetCountAsync();
        return new PagedResultDto<UserProfileDto>(
            totalCount,
            ObjectMapper.Map<List<UserProfile>, List<UserProfileDto>>(userProfiles)
        );
    }

    public async Task<UserProfileDto> UpdateAsync(Guid id, CreateUpdateUserProfileDto input)
    {
        var userProfile = await _userProfileRepository.GetAsync(id);
        userProfile.ExternalUserId = input.ExternalUserId;
        userProfile.Email = input.Email;
        userProfile.FullName = input.FullName;
        userProfile.SystemRole = (SystemRole)input.SystemRole;
        userProfile.StakeholderType = (StakeholderType)input.StakeholderType;
        userProfile.PrimaryRole = input.PrimaryRole;
        userProfile.OrganizationId = input.OrganizationId;
        userProfile.AssignedClients = input.AssignedClients;
        userProfile.JobTitle = input.JobTitle;
        userProfile.Phone = input.Phone;
        userProfile.Country = input.Country;
        userProfile.Location = input.Location;
        userProfile.Department = input.Department;
        userProfile.Bio = input.Bio;
        userProfile.Skills = input.Skills;
        userProfile.Title = input.Title ?? userProfile.Title;
        userProfile.AvatarUrl = input.AvatarUrl ?? userProfile.AvatarUrl;
        userProfile.ManagerId = input.ManagerId ?? userProfile.ManagerId;
        userProfile.StartDate = input.StartDate ?? userProfile.StartDate;
        userProfile.Availability = input.Availability ?? userProfile.Availability;
        userProfile.ProfileCompleted = input.ProfileCompleted;

        await _userProfileRepository.UpdateAsync(userProfile);
        return ObjectMapper.Map<UserProfile, UserProfileDto>(userProfile);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _userProfileRepository.DeleteAsync(id);
    }
}
