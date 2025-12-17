using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Audit.AuditLogs;

public class CreateAuditLogDto
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
    public string? Status { get; set; }

    [StringLength(2000)]
    public string? Changes { get; set; }

    [StringLength(200)]
    public string? IpAddress { get; set; }

    [StringLength(500)]
    public string? UserAgent { get; set; }
}
