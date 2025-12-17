using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.AI.AIRequests;

public class AIRequestDto : FullAuditedEntityDto<Guid>
{
    public string RequestType { get; set; } = string.Empty;
    public string? ModelName { get; set; }
    public string? InputText { get; set; }
    public string? ResponseText { get; set; }
    public string Status { get; set; } = "pending";
    public string? ErrorMessage { get; set; }
    public int? TokensUsed { get; set; }
    public long? ProcessingTimeMs { get; set; }
    public Guid? OrganizationId { get; set; }
    public Guid? TenantId { get; set; }
}
