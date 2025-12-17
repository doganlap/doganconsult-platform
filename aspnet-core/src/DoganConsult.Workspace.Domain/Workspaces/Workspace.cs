using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Workspace.Workspaces;

public class Workspace : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public Guid OrganizationId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "active"; // active|inactive|archived

    [StringLength(500)]
    public string? Settings { get; set; } // JSON-serialized settings

    [StringLength(1000)]
    public string? Members { get; set; } // JSON-serialized member list

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    [StringLength(500)]
    public string? WorkspaceOwner { get; set; } // Owner user ID

    [StringLength(500)]
    public string? Permissions { get; set; } // JSON-serialized permissions model

    public Guid? TenantId { get; set; }

    protected Workspace()
    {
    }

    public Workspace(
        Guid id,
        string code,
        string name,
        Guid organizationId,
        Guid? tenantId = null)
        : base(id)
    {
        Code = code;
        Name = name;
        OrganizationId = organizationId;
        TenantId = tenantId;
    }
}
