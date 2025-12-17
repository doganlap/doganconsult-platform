using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Document.Documents;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Document.Documents;

[Authorize]
public class DocumentAppService : ApplicationService, IDocumentAppService
{
    private readonly IRepository<Document, Guid> _documentRepository;

    public DocumentAppService(IRepository<Document, Guid> documentRepository)
    {
        _documentRepository = documentRepository;
    }

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
            Status = input.Status,
            Version = input.Version,
            ParentDocumentId = input.ParentDocumentId,
            OrganizationId = input.OrganizationId,
            WorkspaceId = input.WorkspaceId
        };

        await _documentRepository.InsertAsync(document);
        return ObjectMapper.Map<Document, DocumentDto>(document);
    }

    public async Task<DocumentDto> GetAsync(Guid id)
    {
        var document = await _documentRepository.GetAsync(id);
        return ObjectMapper.Map<Document, DocumentDto>(document);
    }

    public async Task<PagedResultDto<DocumentDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _documentRepository.GetQueryableAsync();
        var documents = queryable
            .OrderBy(x => x.Name)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount)
            .ToList();

        var totalCount = await _documentRepository.GetCountAsync();
        return new PagedResultDto<DocumentDto>(
            totalCount,
            ObjectMapper.Map<List<Document>, List<DocumentDto>>(documents)
        );
    }

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
        document.Status = input.Status;
        document.Version = input.Version;
        document.ParentDocumentId = input.ParentDocumentId;
        document.OrganizationId = input.OrganizationId;
        document.WorkspaceId = input.WorkspaceId;

        await _documentRepository.UpdateAsync(document);
        return ObjectMapper.Map<Document, DocumentDto>(document);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _documentRepository.DeleteAsync(id);
    }
}
