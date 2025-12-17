using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Audit.AuditLogs;

public class AuditLog : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(200)]
    public string Action { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string EntityType { get; set; } = string.Empty;

    [StringLength(200)]
    public string? EntityId { get; set; }

    [StringLength(200)]
    public string? UserId { get; set; }

    [StringLength(200)]
    public string? UserName { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(50)]
    public string? Status { get; set; } // success|failure|warning

    [StringLength(2000)]
    public string? Changes { get; set; } // JSON string of changes

    [StringLength(200)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }

    public Guid? TenantId { get; set; }

    protected AuditLog()
    {
    }

    public AuditLog(
        Guid id,
        string action,
        string entityType,
        Guid? tenantId = null)
        : base(id)
    {
        Action = action;
        EntityType = entityType;
        TenantId = tenantId;
    }
}
