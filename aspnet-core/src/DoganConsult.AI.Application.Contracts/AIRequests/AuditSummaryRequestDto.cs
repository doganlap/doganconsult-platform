using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.AI.AIRequests;

public class AuditSummaryRequestDto
{
    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    [StringLength(2000)]
    public string Text { get; set; } = string.Empty;
}
