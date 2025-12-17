using System;

namespace DoganConsult.AI.AIRequests;

public class AuditSummaryResponseDto
{
    public string Summary { get; set; } = string.Empty;
    public Guid? RequestId { get; set; }
}
