using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Audit.AuditLogs;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Audit.AuditLogs;

[Authorize]
public class AuditLogAppService : ReadOnlyAppService<AuditLog, AuditLogDto, Guid, PagedAndSortedResultRequestDto>, IAuditLogAppService
{
    private readonly IRepository<AuditLog, Guid> _auditLogRepository;

    public AuditLogAppService(IRepository<AuditLog, Guid> auditLogRepository)
        : base(auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }

    public async Task<AuditLogDto> CreateAsync(CreateAuditLogDto input)
    {
        var auditLog = new AuditLog(
            GuidGenerator.Create(),
            input.Action,
            input.EntityType,
            CurrentTenant.Id
        )
        {
            EntityId = input.EntityId,
            UserId = input.UserId,
            UserName = input.UserName,
            Description = input.Description,
            Status = input.Status,
            Changes = input.Changes,
            IpAddress = input.IpAddress,
            UserAgent = input.UserAgent
        };

        await _auditLogRepository.InsertAsync(auditLog);
        return ObjectMapper.Map<AuditLog, AuditLogDto>(auditLog);
    }

    protected override IQueryable<AuditLog> ApplyDefaultSorting(IQueryable<AuditLog> query)
    {
        return query.OrderByDescending(x => x.CreationTime);
    }
}
