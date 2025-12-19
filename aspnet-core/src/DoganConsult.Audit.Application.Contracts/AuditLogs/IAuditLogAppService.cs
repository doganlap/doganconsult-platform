using System;
using System.Collections.Generic;
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
    Task<List<AuditLogDto>> GetRecentActivitiesAsync(int count = 10);
}
