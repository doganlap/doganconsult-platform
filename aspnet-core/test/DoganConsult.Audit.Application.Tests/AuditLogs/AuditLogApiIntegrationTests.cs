using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Audit;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Audit.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DoganConsult.Audit.AuditLogs;

/// <summary>
/// Integration tests for AuditLog API endpoints
/// </summary>
[Collection(AuditTestConsts.CollectionDefinitionName)]
public class AuditLogApiIntegrationTests : AuditEntityFrameworkCoreTestBase
{
    private readonly IAuditLogAppService _auditLogAppService;

    public AuditLogApiIntegrationTests()
    {
        _auditLogAppService = GetRequiredService<IAuditLogAppService>();
    }

    [Fact]
    public async Task API_Create_Should_Return_Created_AuditLog()
    {
        // Arrange
        var input = new CreateAuditLogDto
        {
            Action = "API_CREATE",
            EntityType = "Organization",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "apitestuser",
            Description = "API test audit log",
            Status = "success"
        };

        // Act
        var result = await _auditLogAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Action.ShouldBe(input.Action);
        result.EntityType.ShouldBe(input.EntityType);
        result.UserName.ShouldBe(input.UserName);
    }

    [Fact]
    public async Task API_GetList_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateAuditLogDto
        {
            Action = "API_CREATE",
            EntityType = "Document",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "apitestuser2",
            Description = "API test audit log 2"
        };
        var input2 = new CreateAuditLogDto
        {
            Action = "API_DELETE",
            EntityType = "Document",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "apitestuser3",
            Description = "API test audit log 3"
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
    }

    [Fact]
    public async Task API_GetRecentActivities_Should_Return_Recent_Activities()
    {
        // Arrange
        var input = new CreateAuditLogDto
        {
            Action = "API_VIEW",
            EntityType = "UserProfile",
            EntityId = Guid.NewGuid().ToString(),
            UserId = Guid.NewGuid().ToString(),
            UserName = "apitestuser4",
            Description = "API test audit log 4"
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
