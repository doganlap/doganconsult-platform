using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Document.Documents;
using DoganConsult.Document.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Document.Documents;

[Authorize]
public class DocumentAppService : ApplicationService, IDocumentAppService
{
    private readonly IRepository<Document, Guid> _documentRepository;
    private readonly DocumentObjectMapper _mapper;

    public DocumentAppService(
        IRepository<Document, Guid> documentRepository,
        DocumentObjectMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    [Authorize(DocumentPermissions.Documents.Create)]
    public async Task<DocumentDto> CreateAsync(CreateUpdateDocumentDto input)
    {
        var document = new Document(
            GuidGenerator.Create(),
            input.Name,
            CurrentTenant.Id
        )
        {
            Description = input.Description,
            FileName = input.FileName,
            FileType = input.FileType,
            FileSize = input.FileSize,
            FilePath = input.FilePath,
            Category = input.Category,
            Status = input.Status ?? "active",
            Version = input.Version,
            ParentDocumentId = input.ParentDocumentId,
            OrganizationId = input.OrganizationId,
            WorkspaceId = input.WorkspaceId,
            DocumentCategory = input.DocumentCategory,
            StoragePath = input.StoragePath,
            UploadedBy = input.UploadedBy ?? CurrentUser.Id,
            UploadDate = input.UploadDate ?? DateTime.UtcNow,
            Tags = input.Tags,
            AccessControl = input.AccessControl,
            Metadata = input.Metadata
        };

        await _documentRepository.InsertAsync(document);
        return _mapper.ToDocumentDto(document);
    }

    [Authorize(DocumentPermissions.Documents.ViewAll)]
    public async Task<DocumentDto> GetAsync(Guid id)
    {
        var document = await _documentRepository.GetAsync(id);
        return _mapper.ToDocumentDto(document);
    }

    [Authorize(DocumentPermissions.Documents.ViewAll)]
    public async Task<PagedResultDto<DocumentDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _documentRepository.GetQueryableAsync();
        var query = queryable
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);
        var documents = await AsyncExecuter.ToListAsync(query);

        var totalCount = await _documentRepository.GetCountAsync();
        return new PagedResultDto<DocumentDto>(
            totalCount,
            _mapper.ToDocumentDtos(documents)
        );
    }

    [Authorize(DocumentPermissions.Documents.Edit)]
    public async Task<DocumentDto> UpdateAsync(Guid id, CreateUpdateDocumentDto input)
    {
        var document = await _documentRepository.GetAsync(id);
        document.Name = input.Name;
        document.Description = input.Description;
        document.FileName = input.FileName;
        document.FileType = input.FileType;
        document.FileSize = input.FileSize;
        document.FilePath = input.FilePath;
        document.Category = input.Category;
        document.Status = input.Status ?? document.Status;
        document.Version = input.Version;
        document.ParentDocumentId = input.ParentDocumentId;
        document.OrganizationId = input.OrganizationId;
        document.WorkspaceId = input.WorkspaceId;
        document.DocumentCategory = input.DocumentCategory ?? document.DocumentCategory;
        document.StoragePath = input.StoragePath ?? document.StoragePath;
        document.Tags = input.Tags ?? document.Tags;
        document.AccessControl = input.AccessControl ?? document.AccessControl;
        document.Metadata = input.Metadata ?? document.Metadata;

        await _documentRepository.UpdateAsync(document);
        return _mapper.ToDocumentDto(document);
    }

    [Authorize(DocumentPermissions.Documents.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _documentRepository.DeleteAsync(id);
    }

    [Authorize(DocumentPermissions.Documents.ViewAll)]
    public async Task<long> GetCountAsync()
    {
        return await _documentRepository.GetCountAsync();
    }
}
