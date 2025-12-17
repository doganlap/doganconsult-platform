using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Document.Documents;

public class DocumentDto : FullAuditedEntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? FileName { get; set; }
    public string? FileType { get; set; }
    public long? FileSize { get; set; }
    public string? FilePath { get; set; }
    public string? Category { get; set; }
    public string Status { get; set; } = "active";
    public int Version { get; set; } = 1;
    public Guid? ParentDocumentId { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? WorkspaceId { get; set; }
    public string? DocumentCategory { get; set; }
    public string? StoragePath { get; set; }
    public Guid? UploadedBy { get; set; }
    public DateTime? UploadDate { get; set; }
    public string? Tags { get; set; }
    public string? AccessControl { get; set; }
    public string? Metadata { get; set; }
    public Guid? TenantId { get; set; }
}
