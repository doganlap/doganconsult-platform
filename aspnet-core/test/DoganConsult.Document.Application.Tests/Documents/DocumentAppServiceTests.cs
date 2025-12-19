using System;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Document;
using DoganConsult.Document.Documents;
using DoganConsult.Document.Domain;
using DoganConsult.Document.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace DoganConsult.Document.Documents;

[Collection(DocumentTestConsts.CollectionDefinitionName)]
public class DocumentAppServiceTests : DocumentEntityFrameworkCoreTestBase
{
    private readonly IDocumentAppService _documentAppService;
    private readonly IRepository<Document, Guid> _documentRepository;

    public DocumentAppServiceTests()
    {
        _documentAppService = GetRequiredService<IDocumentAppService>();
        _documentRepository = GetRequiredService<IRepository<Document, Guid>>();
    }

    [Fact]
    public async Task CreateAsync_Should_Create_Document()
    {
        // Arrange
        var input = new CreateUpdateDocumentDto
        {
            Name = "Test Document",
            Description = "Test Description",
            FileName = "test.pdf",
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
        result.Description.ShouldBe(input.Description);
        result.FileName.ShouldBe(input.FileName);
        result.FileType.ShouldBe(input.FileType);
        result.FileSize.ShouldBe(input.FileSize);
        result.Category.ShouldBe(input.Category);
        result.Status.ShouldBe(input.Status);
    }

    [Fact]
    public async Task GetAsync_Should_Return_Document()
    {
        // Arrange
        var input = new CreateUpdateDocumentDto
        {
            Name = "Test Document 2",
            FileName = "test2.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var created = await _documentAppService.CreateAsync(input);

        // Act
        var result = await _documentAppService.GetAsync(created.Id);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(created.Id);
        result.Name.ShouldBe(input.Name);
        result.FileName.ShouldBe(input.FileName);
    }

    [Fact]
    public async Task GetListAsync_Should_Return_Paged_Results()
    {
        // Arrange
        var input1 = new CreateUpdateDocumentDto
        {
            Name = "Test Document 3",
            FileName = "test3.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var input2 = new CreateUpdateDocumentDto
        {
            Name = "Test Document 4",
            FileName = "test4.pdf",
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
        result.Items.ShouldContain(d => d.Name == input1.Name);
        result.Items.ShouldContain(d => d.Name == input2.Name);
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Document()
    {
        // Arrange
        var createInput = new CreateUpdateDocumentDto
        {
            Name = "Original Name",
            FileName = "original.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        var created = await _documentAppService.CreateAsync(createInput);

        var updateInput = new CreateUpdateDocumentDto
        {
            Name = "Updated Name",
            Description = "Updated Description",
            FileName = "updated.pdf",
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
        result.FileName.ShouldBe(updateInput.FileName);
        result.FileSize.ShouldBe(updateInput.FileSize);
        result.Category.ShouldBe(updateInput.Category);
        result.Status.ShouldBe(updateInput.Status);
    }

    [Fact]
    public async Task DeleteAsync_Should_Delete_Document()
    {
        // Arrange
        var input = new CreateUpdateDocumentDto
        {
            Name = "Test Document 6",
            FileName = "test6.pdf",
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

    [Fact]
    public async Task GetCountAsync_Should_Return_Count()
    {
        // Arrange
        var initialCount = await _documentAppService.GetCountAsync();
        var input = new CreateUpdateDocumentDto
        {
            Name = "Test Document 7",
            FileName = "test7.pdf",
            FileType = "application/pdf",
            Status = "active"
        };
        await _documentAppService.CreateAsync(input);

        // Act
        var result = await _documentAppService.GetCountAsync();

        // Assert
        result.ShouldBeGreaterThan(initialCount);
    }
}
