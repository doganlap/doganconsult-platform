using DoganConsult.Audit.Localization;
using DoganConsult.Audit.AuditLogs;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace DoganConsult.Audit.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AuditController : AbpControllerBase
{
    protected AuditController()
    {
        LocalizationResource = typeof(AuditResource);
    }
}

[Route("api/audit/auditlogs")]
public class AuditLogController : AuditController
{
    private readonly IAuditLogAppService _auditLogAppService;

    public AuditLogController(IAuditLogAppService auditLogAppService)
    {
        _auditLogAppService = auditLogAppService;
    }

    [HttpGet]
    public Task<PagedResultDto<AuditLogDto>> GetListAsync([FromQuery] PagedAndSortedResultRequestDto input)
    {
        return _auditLogAppService.GetListAsync(input);
    }

    [HttpGet("{id}")]
    public Task<AuditLogDto> GetAsync(Guid id)
    {
        return _auditLogAppService.GetAsync(id);
    }

    [HttpPost]
    public Task<AuditLogDto> CreateAsync([FromBody] CreateAuditLogDto input)
    {
        return _auditLogAppService.CreateAsync(input);
    }
}
