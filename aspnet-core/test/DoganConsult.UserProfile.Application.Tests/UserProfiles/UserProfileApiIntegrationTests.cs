using System;
using System.Threading.Tasks;
using DoganConsult.UserProfile;
using DoganConsult.UserProfile.UserProfiles;
using DoganConsult.UserProfile.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DoganConsult.UserProfile.UserProfiles;

/// <summary>
/// Integration tests for UserProfile API endpoints
/// </summary>
[Collection(UserProfileTestConsts.CollectionDefinitionName)]
public class UserProfileApiIntegrationTests : UserProfileEntityFrameworkCoreTestBase
{
    private readonly IUserProfileAppService _userProfileAppService;

    public UserProfileApiIntegrationTests()
    {
        _userProfileAppService = GetRequiredService<IUserProfileAppService>();
    }

    [Fact]
    public async Task API_Create_Should_Return_Created_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "APIEXT001",
            Email = "apitest@example.com",
            FullName = "API Test User",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId,
            JobTitle = "Developer"
        };

        // Act
        var result = await _userProfileAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.ExternalUserId.ShouldBe(input.ExternalUserId);
        result.Email.ShouldBe(input.Email);
        result.FullName.ShouldBe(input.FullName);
    }

    [Fact]
    public async Task API_GetList_Should_Return_Paged_Results()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input1 = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "APIEXT002",
            Email = "apitest2@example.com",
            FullName = "API Test User 2",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var input2 = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "APIEXT003",
            Email = "apitest3@example.com",
            FullName = "API Test User 3",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        await _userProfileAppService.CreateAsync(input1);
        await _userProfileAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10
        };

        // Act
        var result = await _userProfileAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task API_Update_Should_Update_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var createInput = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "APIEXT004",
            Email = "original@example.com",
            FullName = "Original Name",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var created = await _userProfileAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "APIEXT004",
            Email = "updated@example.com",
            FullName = "Updated Name",
            SystemRole = SystemRoleDto.Admin,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId,
            JobTitle = "Senior Developer"
        };

        // Act
        var result = await _userProfileAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Email.ShouldBe(updateInput.Email);
        result.FullName.ShouldBe(updateInput.FullName);
        result.SystemRole.ShouldBe(updateInput.SystemRole);
    }
}
