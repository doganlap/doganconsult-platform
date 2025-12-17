using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Audit.AuditLogs;

public class AuditLogDto : FullAuditedEntityDto<Guid>
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string? EntityId { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Changes { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public Guid? TenantId { get; set; }
}
