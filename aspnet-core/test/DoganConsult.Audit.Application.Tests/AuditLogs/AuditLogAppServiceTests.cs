using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Audit;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Audit.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DoganConsult.Audit.AuditLogs;

[Collection(AuditTestConsts.CollectionDefinitionName)]
public class AuditLogAppServiceTests : AuditEntityFrameworkCoreTestBase
{
    private readonly IAuditLogAppService _auditLogAppService;
    private readonly IRepository<AuditLog, Guid> _auditLogRepository;

    public AuditLogAppServiceTests()
    {
        _auditLogAppService = GetRequiredService<IAuditLogAppService>();
        _auditLogRepository = GetRequiredService<IRepository<AuditLog, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_AuditLog()
    {
        // Arrange
        var input = new CreateAuditLogDto
        {
            Action = "CREATE",
            EntityType = "Organization",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser",
            Description = "Test audit log",
            Status = "success"
        };

        // Act
        var result = await _auditLogAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Action.ShouldBe(input.Action);
        result.EntityType.ShouldBe(input.EntityType);
        result.EntityId.ShouldBe(input.EntityId);
        result.UserId.ShouldBe(input.UserId);
        result.UserName.ShouldBe(input.UserName);
        result.Description.ShouldBe(input.Description);
        result.Status.ShouldBe(input.Status);
    }

    [Fact]
    public async Task GetAsync_Should_Return_AuditLog()
    {
        // Arrange
        var input = new CreateAuditLogDto
        {
            Action = "UPDATE",
            EntityType = "Workspace",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser2",
            Description = "Test audit log 2"
        };
        var created = await _auditLogAppService.CreateAsync(input);

        // Act
        var result = await _auditLogAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Action.ShouldBe(input.Action);
        result.EntityType.ShouldBe(input.EntityType);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateAuditLogDto
        {
            Action = "CREATE",
            EntityType = "Document",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser3",
            Description = "Test audit log 3"
        };
        var input2 = new CreateAuditLogDto
        {
            Action = "DELETE",
            EntityType = "Document",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser4",
            Description = "Test audit log 4"
        };
        await _auditLogAppService.CreateAsync(input1);
        await _auditLogAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10
        };

        // Act
        var result = await _auditLogAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.ShouldContain(a => a.Action == input1.Action);
        result.Items.ShouldContain(a => a.Action == input2.Action);
    }

    [Fact]
    public async Task GetRecentActivitiesAsync_Should_Return_Recent_Activities()
    {
        // Arrange
        var input = new CreateAuditLogDto
        {
            Action = "VIEW",
            EntityType = "UserProfile",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "testuser5",
            Description = "Test audit log 5"
        };
        await _auditLogAppService.CreateAsync(input);

        // Act
        var result = await _auditLogAppService.GetRecentActivitiesAsync(10);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThan(0);
        result.ShouldContain(a => a.Action == input.Action);
    }
}
