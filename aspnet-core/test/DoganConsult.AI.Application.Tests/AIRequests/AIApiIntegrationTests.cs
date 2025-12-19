using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.AI;
using DoganConsult.AI.AIRequests;
using DoganConsult.AI.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace DoganConsult.AI.AIRequests;

/// <summary>
/// Integration tests for AI API endpoints
/// </summary>
[Collection(AITestConsts.CollectionDefinitionName)]
public class AIApiIntegrationTests : AIEntityFrameworkCoreTestBase
{
    private readonly IAIAppService _aiAppService;

    public AIApiIntegrationTests()
    {
        _aiAppService = GetRequiredService<IAIAppService>();
    }

    [Fact]
    public async Task API_GenerateAuditSummary_Should_Return_Summary()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var input = new AuditSummaryRequestDto
        {
            OrganizationId = organizationId,
            Text = "This is a comprehensive audit text that contains multiple findings and recommendations that need to be summarized into a concise format for executive review."
        };

        // Act
        var result = await _aiAppService.GenerateAuditSummaryAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.RequestId.ShouldNotBeNull();
        result.Summary.ShouldNotBeNullOrEmpty();
        result.Summary.Length.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task API_GenerateAuditSummary_With_Long_Text_Should_Handle_Correctly()
    {
        // Arrange
        var organizationId = Guid.NewGuid();
        var longText = string.Join(" ", Enumerable.Range(1, 100).Select(i => $"Paragraph {i} with detailed audit information."));
        var input = new AuditSummaryRequestDto
        {
            OrganizationId = organizationId,
            Text = longText
        };

        // Act
        var result = await _aiAppService.GenerateAuditSummaryAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Summary.ShouldNotBeNullOrEmpty();
    }
}
