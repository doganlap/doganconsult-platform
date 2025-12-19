using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.UserProfile;
using DoganConsult.UserProfile.UserProfiles;
using DoganConsult.UserProfile.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DoganConsult.UserProfile.UserProfiles;

[Collection(UserProfileTestConsts.CollectionDefinitionName)]
public class UserProfileAppServiceTests : UserProfileEntityFrameworkCoreTestBase
{
    private readonly IUserProfileAppService _userProfileAppService;
    private readonly IRepository<UserProfile, Guid> _userProfileRepository;

    public UserProfileAppServiceTests()
    {
        _userProfileAppService = GetRequiredService<IUserProfileAppService>();
        _userProfileRepository = GetRequiredService<IRepository<UserProfile, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT001",
            Email = "test@example.com",
            FullName = "Test User",
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
        result.SystemRole.ShouldBe(input.SystemRole);
        result.OrganizationId.ShouldBe(input.OrganizationId);
    }

    [Fact]
    public async Task GetAsync_Should_Return_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT002",
            Email = "test2@example.com",
            FullName = "Test User 2",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var created = await _userProfileAppService.CreateAsync(input);

        // Act
        var result = await _userProfileAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Email.ShouldBe(input.Email);
        result.FullName.ShouldBe(input.FullName);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input1 = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT003",
            Email = "test3@example.com",
            FullName = "Test User 3",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var input2 = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT004",
            Email = "test4@example.com",
            FullName = "Test User 4",
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
    public async Task UpdateAsync_Should_Update_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var createInput = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT005",
            Email = "original@example.com",
            FullName = "Original Name",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var created = await _userProfileAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT005",
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

    [Fact]
    public async Task DeleteAsync_Should_Delete_UserProfile()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateUserProfileDto
        {
            ExternalUserId = "EXT006",
            Email = "test6@example.com",
            FullName = "Test User 6",
            SystemRole = SystemRoleDto.User,
            StakeholderType = StakeholderTypeDto.DemoGuest,
            OrganizationId = organizationId
        };
        var created = await _userProfileAppService.CreateAsync(input);

        // Act
        await _userProfileAppService.DeleteAsync(created.Id);

        // Assert
        var exception = await Should.ThrowAsync<Exception>(async () =>
        {
            await _userProfileAppService.GetAsync(created.Id);
        });
    }
}
