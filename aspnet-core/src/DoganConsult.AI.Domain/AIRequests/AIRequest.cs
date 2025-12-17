using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.AI.AIRequests;

public class AIRequest : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(200)]
    public string RequestType { get; set; } = string.Empty; // audit-summary, document-analysis, etc.

    [StringLength(200)]
    public string? ModelName { get; set; }

    [StringLength(2000)]
    public string? InputText { get; set; }

    [StringLength(10000)]
    public string? ResponseText { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = "pending"; // pending|completed|failed

    [StringLength(2000)]
    public string? ErrorMessage { get; set; }

    public int? TokensUsed { get; set; }

    public long? ProcessingTimeMs { get; set; }

    public Guid? OrganizationId { get; set; }

    public Guid? TenantId { get; set; }

    protected AIRequest()
    {
    }

    public AIRequest(
        Guid id,
        string requestType,
        Guid? tenantId = null)
        : base(id)
    {
        RequestType = requestType;
        TenantId = tenantId;
    }
}
