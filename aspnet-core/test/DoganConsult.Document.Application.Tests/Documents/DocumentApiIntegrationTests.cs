using System;
using System.Threading.Tasks;
using DoganConsult.Document;
using DoganConsult.Document.Documents;
using DoganConsult.Document.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Xunit;

namespace DoganConsult.Document.Documents;

/// <summary>
/// Integration tests for Document API endpoints
/// </summary>
[Collection(DocumentTestConsts.CollectionDefinitionName)]
public class DocumentApiIntegrationTests : DocumentEntityFrameworkCoreTestBase
{
    private readonly IDocumentAppService _documentAppService;

    public DocumentApiIntegrationTests()
    {
        _documentAppService = GetRequiredService<IDocumentAppService>();
    }

    [Fact]
    public async Task API_Create_Should_Return_Created_Document()
    {
        // Arrange
        var input = new CreateUpdateDocumentDto
        {
            Name = "API Test Document",
            Description = "Test Description",
            FileName = "test-api.pdf",
            FileType = "application/pdf",
            FileSize = 1024,
            Category = "Test Category",
            Status = "active"
        };

        // Act
        var result = await _documentAppService.CreateAsync(input);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe(input.Name);
        result.FileName.ShouldBe(input.FileName);
        result.FileType.ShouldBe(input.FileType);
    }

    [Fact]
    public async Task API_GetList_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateUpdateDocumentDto
        {
            Name = "API Test Document 2",
            FileName = "test-api2.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var input2 = new CreateUpdateDocumentDto
        {
            Name = "API Test Document 3",
            FileName = "test-api3.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        await _documentAppService.CreateAsync(input1);
        await _documentAppService.CreateAsync(input2);

        var listInput = new PagedAndSortedResultRequestDto
        {
            SkipCount = 0,
            MaxResultCount = 10
        };

        // Act
        var result = await _documentAppService.GetListAsync(listInput);

        // Assert
        result.ShouldNotBeNull();
        result.TotalCount.ShouldBeGreaterThanOrEqualTo(2);
        result.Items.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    [Fact]
    public async Task API_Update_Should_Update_Document()
    {
        // Arrange
        var createInput = new CreateUpdateDocumentDto
        {
            Name = "Original Name",
            FileName = "original-api.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var created = await _documentAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateDocumentDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            FileName = "updated-api.pdf",
            FileType = "application/pdf",
            FileSize = 2048,
            Category = "Updated Category",
            Status = "inactive"
        };

        // Act
        var result = await _documentAppService.UpdateAsync(created.Id, updateInput);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(updateInput.Name);
        result.Description.ShouldBe(updateInput.Description);
        result.Status.ShouldBe(updateInput.Status);
    }

    [Fact]
    public async Task API_Delete_Should_Remove_Document()
    {
        // Arrange
        var input = new CreateUpdateDocumentDto
        {
            Name = "API Test Document 4",
            FileName = "test-api4.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var created = await _documentAppService.CreateAsync(input);

        // Act
        await _documentAppService.DeleteAsync(created.Id);

        // Assert
        var exception = await Should.ThrowAsync<Exception>(async () =>
        {
            await _documentAppService.GetAsync(created.Id);
        });
    }
}
