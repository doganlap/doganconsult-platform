using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Organization.Organizations;

public class Organization : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public OrganizationType Type { get; set; }

    [StringLength(100)]
    public string? Sector { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    /// <summary>
    /// Comma-separated list of regulator names
    /// </summary>
    [StringLength(500)]
    public string? Regulators { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "active"; // active|pilot|trial|inactive

    [StringLength(200)]
    [EmailAddress]
    public string? PrimaryContactEmail { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }

    public Guid? TenantId { get; set; }

    protected Organization()
    {
    }

    public Organization(
        Guid id,
        string code,
        string name,
        OrganizationType type,
        Guid? tenantId = null)
        : base(id)
    {
        Code = code;
        Name = name;
        Type = type;
        TenantId = tenantId;
    }
}
