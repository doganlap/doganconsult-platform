using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace DoganConsult.Audit.AuditLogs;

public interface IAuditLogAppService : IReadOnlyAppService<
    AuditLogDto,
    Guid,
    PagedAndSortedResultRequestDto>
{
    Task<AuditLogDto> CreateAsync(CreateAuditLogDto input);
}
