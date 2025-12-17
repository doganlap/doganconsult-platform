using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Audit.Approvals;

public class ProcessApprovalDto
{
    [Required]
    public Guid ApprovalRequestId { get; set; }

    [Required]
    public bool Approve { get; set; }

    [StringLength(2000)]
    public string? Comments { get; set; }
}
