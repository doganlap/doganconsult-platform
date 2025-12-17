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
    public string? StreetAddress { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string LegalName { get; set; } = string.Empty;
    public string? RegistrationNumber { get; set; }
    public string? VatNumber { get; set; }
    public string? BillingAddress { get; set; }
    public DateTime? IncorporationDate { get; set; }
    public string? AuthorizedPersonName { get; set; }
    public string? AuthorizedPersonEmail { get; set; }
    public string? BusinessDescription { get; set; }
    public string? TechnologyScope { get; set; }
    public string? ServiceScope { get; set; }
    public string? Regulators { get; set; }
    public string Status { get; set; } = "active";
    public string? PrimaryContactEmail { get; set; }
    public string? Notes { get; set; }
    public Guid? TenantId { get; set; }
}
