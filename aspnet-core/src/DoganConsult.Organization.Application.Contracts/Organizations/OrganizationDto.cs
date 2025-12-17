using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Organization.Organizations;

public class OrganizationDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public OrganizationTypeDto Type { get; set; }
    public string? Sector { get; set; }
    public string? Country { get; set; }
    public string? City { get; set; }
    public string? Regulators { get; set; }
    public string Status { get; set; } = "active";
    public string? PrimaryContactEmail { get; set; }
    public string? Notes { get; set; }
    public Guid? TenantId { get; set; }
}
