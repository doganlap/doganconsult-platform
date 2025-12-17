using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Organization.Organizations;

public class CreateUpdateOrganizationDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public OrganizationTypeDto Type { get; set; }

    [StringLength(100)]
    public string? Sector { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(100)]
    public string? City { get; set; }

    [StringLength(255)]
    public string? StreetAddress { get; set; }

    [StringLength(100)]
    public string? State { get; set; }

    [StringLength(20)]
    public string? PostalCode { get; set; }

    [Required]
    [StringLength(255)]
    public string LegalName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? RegistrationNumber { get; set; }

    [StringLength(100)]
    public string? VatNumber { get; set; }

    [StringLength(500)]
    public string? BillingAddress { get; set; }

    public DateTime? IncorporationDate { get; set; }

    [StringLength(255)]
    public string? AuthorizedPersonName { get; set; }

    [StringLength(255)]
    [EmailAddress]
    public string? AuthorizedPersonEmail { get; set; }

    [StringLength(1000)]
    public string? BusinessDescription { get; set; }

    [StringLength(500)]
    public string? TechnologyScope { get; set; }

    [StringLength(500)]
    public string? ServiceScope { get; set; }

    [StringLength(500)]
    public string? Regulators { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "active";

    [StringLength(200)]
    [EmailAddress]
    public string? PrimaryContactEmail { get; set; }

    [StringLength(2000)]
    public string? Notes { get; set; }
}
