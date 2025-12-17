using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.Document.Documents;

public class Document : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(200)]
    public string? FileName { get; set; }

    [StringLength(50)]
    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    [StringLength(1000)]
    public string? FilePath { get; set; }

    [StringLength(200)]
    public string? Category { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "active"; // active|archived|deleted

    public int Version { get; set; } = 1;

    public Guid? ParentDocumentId { get; set; } // For versioning

    public Guid? OrganizationId { get; set; }

    public Guid? WorkspaceId { get; set; }

    public Guid? TenantId { get; set; }

    protected Document()
    {
    }

    public Document(
        Guid id,
        string name,
        Guid? tenantId = null)
        : base(id)
    {
        Name = name;
        TenantId = tenantId;
    }
}
