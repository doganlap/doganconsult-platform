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
