using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Audit.AuditLogs;
using DoganConsult.Audit.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace DoganConsult.Audit.AuditLogs;

[Authorize(AuditPermissions.AuditLogs.ViewAll)]
public class AuditLogAppService : ReadOnlyAppService<AuditLog, AuditLogDto, Guid, PagedAndSortedResultRequestDto>, IAuditLogAppService
{
    private readonly IRepository<AuditLog, Guid> _auditLogRepository;
    private readonly AuditApplicationMappers _mapper;

    public AuditLogAppService(
        IRepository<AuditLog, Guid> auditLogRepository,
        AuditApplicationMappers mapper)
        : base(auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
    }

    [Authorize(AuditPermissions.AuditLogs.ViewAll)]
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
        return _mapper.ToAuditLogDto(auditLog);
    }

    protected override IQueryable<AuditLog> ApplyDefaultSorting(IQueryable<AuditLog> query)
    {
        return query.OrderByDescending(x => x.CreationTime);
    }

    protected override AuditLogDto MapToGetOutputDto(AuditLog entity)
    {
        return _mapper.ToAuditLogDto(entity);
    }

    public override async Task<PagedResultDto<AuditLogDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _auditLogRepository.GetQueryableAsync();
        var query = ApplyDefaultSorting(queryable)
            .Skip(input.SkipCount)
            .Take(input.MaxResultCount);

        var entities = await AsyncExecuter.ToListAsync(query);
        var totalCount = await AsyncExecuter.CountAsync(queryable);

        return new PagedResultDto<AuditLogDto>(
            totalCount,
            _mapper.ToAuditLogDtoList(entities)
        );
    }

    [Authorize(AuditPermissions.AuditLogs.ViewAll)]
    public async Task<List<AuditLogDto>> GetRecentActivitiesAsync(int count = 10)
    {
        var queryable = await _auditLogRepository.GetQueryableAsync();
        var activities = queryable
            .OrderByDescending(x => x.CreationTime)
            .Take(count)
            .ToList();
        
        return _mapper.ToAuditLogDtoList(activities);
    }
}
