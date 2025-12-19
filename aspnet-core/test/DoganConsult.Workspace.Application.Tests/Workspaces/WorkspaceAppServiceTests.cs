using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Workspace;
using DoganConsult.Workspace.Workspaces;
using DoganConsult.Workspace.Domain;
using DoganConsult.Workspace.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DoganConsult.Workspace.Workspaces;

[Collection(WorkspaceTestConsts.CollectionDefinitionName)]
public class WorkspaceAppServiceTests : WorkspaceEntityFrameworkCoreTestBase
{
    private readonly IWorkspaceAppService _workspaceAppService;
    private readonly IRepository<Workspace, Guid> _workspaceRepository;

    public WorkspaceAppServiceTests()
    {
        _workspaceAppService = GetRequiredService<IWorkspaceAppService>();
        _workspaceRepository = GetRequiredService<IRepository<Workspace, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WS001",
            Name = "Test Workspace",
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
        result.Description.ShouldBe(input.Description);
        result.Status.ShouldBe(input.Status);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WS002",
            Name = "Test Workspace 2",
            OrganizationId = organizationId,
            Status = "active"
        };
        var created = await _workspaceAppService.CreateAsync(input);

        // Act
        var result = await _workspaceAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Code.ShouldBe(input.Code);
        result.Name.ShouldBe(input.Name);
        result.OrganizationId.ShouldBe(input.OrganizationId);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input1 = new CreateUpdateWorkspaceDto
        {
            Code = "WS003",
            Name = "Test Workspace 3",
            OrganizationId = organizationId,
            Status = "active"
        };
        var input2 = new CreateUpdateWorkspaceDto
        {
            Code = "WS004",
            Name = "Test Workspace 4",
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
        result.Items.ShouldContain(w => w.Code == input1.Code);
        result.Items.ShouldContain(w => w.Code == input2.Code);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var createInput = new CreateUpdateWorkspaceDto
        {
            Code = "WS005",
            Name = "Original Name",
            OrganizationId = organizationId,
            Status = "active"
        };
        var created = await _workspaceAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateWorkspaceDto
        {
            Code = "WS005-UPDATED",
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
        result.Description.ShouldBe(updateInput.Description);
        result.Status.ShouldBe(updateInput.Status);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Workspace()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WS006",
            Name = "Test Workspace 6",
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

    [Fact]
    public async Task GetCountAsync_Should_Return_Count()
    {
        // Arrange
        var initialCount = await _workspaceAppService.GetCountAsync();
        var organizationId = Guid.NewGuid();
        var input = new CreateUpdateWorkspaceDto
        {
            Code = "WS007",
            Name = "Test Workspace 7",
            OrganizationId = organizationId,
            Status = "active"
        };
        await _workspaceAppService.CreateAsync(input);

        // Act
        var result = await _workspaceAppService.GetCountAsync();

        // Assert
        result.ShouldBeGreaterThan(initialCount);
    }
}
