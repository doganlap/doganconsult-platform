using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization;
using DoganConsult.Organization.Organizations;
using DoganConsult.Organization.Domain;
using DoganConsult.Organization.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DoganConsult.Organization.Organizations;

[Collection(OrganizationTestConsts.CollectionDefinitionName)]
public class OrganizationAppServiceTests : OrganizationEntityFrameworkCoreTestBase
{
    private readonly IOrganizationAppService _organizationAppService;
    private readonly IRepository<Organization, Guid> _organizationRepository;

    public OrganizationAppServiceTests()
    {
        _organizationAppService = GetRequiredService<IOrganizationAppService>();
        _organizationRepository = GetRequiredService<IRepository<Organization, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Organization()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "TEST001",
            Name = "Test Organization",
            LegalName = "Test Organization Legal Name",
            Type = OrganizationTypeDto.Client,
            Sector = "Technology",
            Country = "USA",
            City = "San Francisco",
            Status = "active",
            PrimaryContactEmail = "test@example.com"
        };

        // Act
        OrganizationDto? result = null;
        try
        {
            result = await _organizationAppService.CreateAsync(input);
        }
        catch (Volo.Abp.Validation.AbpValidationException ex)
        {
            // Capture validation errors for debugging
            var errors = string.Join("; ", ex.ValidationErrors.Select(e => 
                $"{string.Join(", ", e.MemberNames ?? Array.Empty<string>())}: {e.ErrorMessage}"));
            throw new Exception($"Validation failed: {errors}", ex);
        }

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
        result.Type.ShouldBe(input.Type);
        result.Sector.ShouldBe(input.Sector);
        result.Country.ShouldBe(input.Country);
        result.City.ShouldBe(input.City);
        result.Status.ShouldBe(input.Status);
        result.PrimaryContactEmail.ShouldBe(input.PrimaryContactEmail);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Organization()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "TEST002",
            Name = "Test Organization 2",
            LegalName = "Test Organization 2 Legal",
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
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateUpdateOrganizationDto
        {
            Code = "TEST003",
            Name = "Test Organization 3",
            LegalName = "Test Organization 3 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var input2 = new CreateUpdateOrganizationDto
        {
            Code = "TEST004",
            Name = "Test Organization 4",
            LegalName = "Test Organization 4 Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        await _organizationAppService.CreateAsync(input1);
        await _organizationAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10
        };

        // Act
        var result = await _organizationAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldContain(o => o.Code == input1.Code);
        result.Items.ShouldContain(o => o.Code == input2.Code);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Organization()
    {
        // Arrange
        var createInput = new CreateUpdateOrganizationDto
        {
            Code = "TEST005",
            Name = "Original Name",
            LegalName = "Original Name Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active"
        };
        var created = await _organizationAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateOrganizationDto
        {
            Code = "TEST005-UPDATED",
            Name = "Updated Name",
            LegalName = "Updated Name Legal",
            Type = OrganizationTypeDto.Client,
            Status = "inactive",
            Sector = "Finance",
            Country = "Canada",
            City = "Toronto"
        };

        // Act
        var result = await _organizationAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Code.ShouldBe(updateInput.Code);
        result.Name.ShouldBe(updateInput.Name);
        result.Status.ShouldBe(updateInput.Status);
        result.Sector.ShouldBe(updateInput.Sector);
        result.Country.ShouldBe(updateInput.Country);
        result.City.ShouldBe(updateInput.City);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Organization()
    {
        // Arrange
        var input = new CreateUpdateOrganizationDto
        {
            Code = "TEST006",
            Name = "Test Organization 6",
            LegalName = "Test Organization 6 Legal",
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
    public async Task GetCountAsync_Should_Return_Count()
    {
        // Arrange
        var initialCount = await _organizationAppService.GetCountAsync();
        var input = new CreateUpdateOrganizationDto
        {
            Code = "TEST007",
            Name = "Test Organization 7",
            LegalName = "Test Organization 7 Legal",
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
    public async Task GetStatisticsAsync_Should_Return_Statistics()
    {
        // Arrange
        var input1 = new CreateUpdateOrganizationDto
        {
            Code = "TEST008",
            Name = "Active Org",
            LegalName = "Active Org Legal",
            Type = OrganizationTypeDto.Client,
            Status = "active",
            Sector = "Technology",
            Country = "USA"
        };
        var input2 = new CreateUpdateOrganizationDto
        {
            Code = "TEST009",
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
        result.Trends.ShouldNotBeNull();
    }
}
