using System;
using System.Threading.Tasks;
using DoganConsult.Organization;
using DoganConsult.Organization.Organizations;
using DoganConsult.Organization.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DoganConsult.Organization.Organizations;

/// <summary>
/// Integration tests for Organization API endpoints
/// These tests verify the API controller layer works correctly with the application service
/// </summary>
[Collection(OrganizationTestConsts.CollectionDefinitionName)]
public class OrganizationApiIntegrationTests : OrganizationEntityFrameworkCoreTestBase
{
    private readonly IOrganizationAppService _organizationAppService;

    public OrganizationApiIntegrationTests()
    {
        _organizationAppService = GetRequiredService<IOrganizationAppService>();
    }

    [Fact]
    public async Task API_Create_Should_Return_Created_Organization()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "API001",
            Name = "API Test Organization",
            LegalName = "API Test Organization Legal",
            Type = OrganizationTypeDto.Client,
            Sector = "Technology",
            Country = "USA",
            City = "Seattle",
            Status = "active",
            PrimaryContactEmail = "api@example.com"
        };

        // Act
        var result = await _organizationAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
        result.Type.ShouldBe(input.Type);
    }

    [Fact]
    public async Task API_GetList_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateUpdateOrganizationDto
        {
            Code = "API002",
            Name = "API Test Org 2",
            LegalName = "API Test Org 2 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var input2 = new CreateUpdateOrganizationDto
        {
            Code = "API003",
            Name = "API Test Org 3",
            LegalName = "API Test Org 3 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        await _organizationAppService.CreateAsync(input1);
        await _organizationAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10,
            Sorting = "Name"
        };

        // Act
        var result = await _organizationAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task API_Get_Should_Return_Organization_By_Id()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "API004",
            Name = "API Test Org 4",
            LegalName = "API Test Org 4 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var created = await _organizationAppService.CreateAsync(input);

        // Act
        var result = await _organizationAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
    }

    [Fact]
    public async Task API_Update_Should_Update_Organization()
    {
        // Arrange
        var createInput = new CreateUpdateOrganizationDto
        {
            Code = "API005",
            Name = "Original Name",
            LegalName = "Original Name Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var created = await _organizationAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateOrganizationDto
        {
            Code = "API005-UPDATED",
            Name = "Updated Name",
            LegalName = "Updated Name Legal",
            Type = OrganizationTypeDto.Client,
            Status = "inactive",
            Sector = "Finance"
        };

        // Act
        var result = await _organizationAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Code.ShouldBe(updateInput.Code);
        result.Name.ShouldBe(updateInput.Name);
        result.Status.ShouldBe(updateInput.Status);
    }

    [Fact]
    public async Task API_Delete_Should_Remove_Organization()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "API006",
            Name = "API Test Org 6",
            LegalName = "API Test Org 6 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var created = await _organizationAppService.CreateAsync(input);

        // Act
        await _organizationAppService.DeleteAsync(created.Id);

        // Assert
        var exception = await Should.ThrowAsync<Exception>(async () =>
        {
            await _organizationAppService.GetAsync(created.Id);
        });
    }

    [Fact]
    public async Task API_GetCount_Should_Return_Total_Count()
    {
        // Arrange
        var initialCount = await _organizationAppService.GetCountAsync();
        var input = new CreateUpdateOrganizationDto
        {
            Code = "API007",
            Name = "API Test Org 7",
            LegalName = "API Test Org 7 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        await _organizationAppService.CreateAsync(input);

        // Act
        var result = await _organizationAppService.GetCountAsync();

        // Assert
        result.ShouldBeGreaterThan(initialCount);
    }

    [Fact]
    public async Task API_GetStatistics_Should_Return_Statistics()
    {
        // Arrange
        var input1 = new CreateUpdateOrganizationDto
        {
            Code = "API008",
            Name = "Active Org",
            LegalName = "Active Org Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active",
            Sector = "Technology",
            Country = "USA"
        };
        var input2 = new CreateUpdateOrganizationDto
        {
            Code = "API009",
            Name = "Inactive Org",
            LegalName = "Inactive Org Legal",
            Type = OrganizationTypeDto.Client,
            Status = "inactive",
            Sector = "Finance",
            Country = "Canada"
        };
        await _organizationAppService.CreateAsync(input1);
        await _organizationAppService.CreateAsync(input2);

        // Act
        var result = await _organizationAppService.GetStatisticsAsync();

        // Assert
        result.ShouldNotBeNull();
        result.TotalOrganizations.ShouldBeGreaterThanOrEqualTo(2);
        result.ActiveOrganizations.ShouldBeGreaterThanOrEqualTo(1);
        result.InactiveOrganizations.ShouldBeGreaterThanOrEqualTo(1);
        result.ByType.ShouldNotBeNull();
        result.BySector.ShouldNotBeNull();
        result.ByCountry.ShouldNotBeNull();
    }
}
