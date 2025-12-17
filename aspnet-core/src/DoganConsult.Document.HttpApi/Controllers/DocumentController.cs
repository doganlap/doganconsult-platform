using DoganConsult.Document.Localization;
using DoganConsult.Document.Documents;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Document.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class DocumentController : AbpControllerBase
{
    protected DocumentController()
    {
        LocalizationResource = typeof(DocumentResource);
    }
}

[Route("api/document/documents")]
public class DocumentApiController : DocumentController
{
    private readonly IDocumentAppService _documentAppService;

    public DocumentApiController(IDocumentAppService documentAppService)
    {
        _documentAppService = documentAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<DocumentDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _documentAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<DocumentDto> GetAsync(Guid id)
    {
        return _documentAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<DocumentDto> CreateAsync([FromBody] CreateUpdateDocumentDto input)
    {
        return _documentAppService.CreateAsync(input);
    }

    [HttpPut("{id}")]
    public Task<DocumentDto> UpdateAsync(Guid id, [FromBody] CreateUpdateDocumentDto input)
    {
        return _documentAppService.UpdateAsync(id, input);
    }

    [HttpDelete("{id}")]
    public Task DeleteAsync(Guid id)
    {
        return _documentAppService.DeleteAsync(id);
    }
}
