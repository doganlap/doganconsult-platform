using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Document.Documents;

public class CreateUpdateDocumentDto
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
    public string Status { get; set; } = "active";

    public int Version { get; set; } = 1;

    public Guid? ParentDocumentId { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? WorkspaceId { get; set; }
}
