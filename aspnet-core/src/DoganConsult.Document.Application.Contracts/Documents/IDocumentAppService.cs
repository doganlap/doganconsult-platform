using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.Document.Documents;

public interface IDocumentAppService : ICrudAppService<
    DocumentDto,
    Guid,
    PagedAndSortedResultRequestDto,
    CreateUpdateDocumentDto,
    CreateUpdateDocumentDto>
{
    Task<long> GetCountAsync();
}
