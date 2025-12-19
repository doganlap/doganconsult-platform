using System;
using System.Threading.Tasks;
using DoganConsult.Workspace;
using DoganConsult.Workspace.Workspaces;
using DoganConsult.Workspace.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DoganConsult.Workspace.Workspaces;

/// <summary>
/// Integration tests for Workspace API endpoints
/// </summary>
[Collection(WorkspaceTestConsts.CollectionDefinitionName)]
public class WorkspaceApiIntegrationTests : WorkspaceEntityFrameworkCoreTestBase
{
    private readonly IWorkspaceAppService _workspaceAppService;

    public WorkspaceApiIntegrationTests()
    {
        _workspaceAppService = GetRequiredService<IWorkspaceAppService>();
    }

    [Fact]
    public async Task API_Create_Should_Return_Created_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI001",
            Name = "API Test Workspace",
            OrganizationId = organizationId,
            Description = "Test Description",
            Status = "active"
        };

        // Act
        var result = await _workspaceAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
        result.OrganizationId.ShouldBe(input.OrganizationId);
    }

    [Fact]
    public async Task API_GetList_Should_Return_Paged_Results()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input1 = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI002",
            Name = "API Test Workspace 2",
            OrganizationId = organizationId,
            Status = "active"
        };
        var input2 = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI003",
            Name = "API Test Workspace 3",
            OrganizationId = organizationId,
            Status = "active"
        };
        await _workspaceAppService.CreateAsync(input1);
        await _workspaceAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10
        };

        // Act
        var result = await _workspaceAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task API_Update_Should_Update_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var createInput = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI004",
            Name = "Original Name",
            OrganizationId = organizationId,
            Status = "active"
        };
        var created = await _workspaceAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI004-UPDATED",
            Name = "Updated Name",
            OrganizationId = organizationId,
            Description = "Updated Description",
            Status = "inactive"
        };

        // Act
        var result = await _workspaceAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Code.ShouldBe(updateInput.Code);
        result.Name.ShouldBe(updateInput.Name);
        result.Status.ShouldBe(updateInput.Status);
    }

    [Fact]
    public async Task API_Delete_Should_Remove_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WSAPI005",
            Name = "API Test Workspace 5",
            OrganizationId = organizationId,
            Status = "active"
        };
        var created = await _workspaceAppService.CreateAsync(input);

        // Act
        await _workspaceAppService.DeleteAsync(created.Id);

        // Assert
        var exception = await Should.ThrowAsync<Exception>(async () =>
        {
            await _workspaceAppService.GetAsync(created.Id);
        });
    }
}
