using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Web.Blazor.Organizations;

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string OrganizationCode { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public OrganizationType OrganizationType { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Sector { get; set; } = string.Empty;
    public string RegulatoryRequirements { get; set; } = string.Empty;
    public string ContactInformation { get; set; } = string.Empty;
    public OrganizationStatus OrganizationStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = string.Empty;
    public DateTime? LastModifiedAt { get; set; }
    public string? LastModifiedBy { get; set; }
}

public class CreateUpdateOrganizationDto
{
    [Required(ErrorMessage = "Organization Code is required")]
    [StringLength(20, MinimumLength = 2, ErrorMessage = "Organization Code must be between 2 and 20 characters")]
    [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Organization Code can only contain letters, numbers, hyphens, and underscores")]
    public string OrganizationCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Organization Name is required")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Organization Name must be between 2 and 200 characters")]
    public string OrganizationName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Organization Type is required")]
    public OrganizationType OrganizationType { get; set; }

    [StringLength(100, ErrorMessage = "Country must not exceed 100 characters")]
    public string Country { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "City must not exceed 100 characters")]
    public string City { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Sector must not exceed 100 characters")]
    public string Sector { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Regulatory Requirements must not exceed 1000 characters")]
    public string RegulatoryRequirements { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Contact Information must not exceed 500 characters")]
    public string ContactInformation { get; set; } = string.Empty;

    [Required(ErrorMessage = "Organization Status is required")]
    public OrganizationStatus OrganizationStatus { get; set; }
}

public enum OrganizationType
{
    Internal = 0,
    Client = 1,
    Regulator = 2,
    Demo = 3,
    Other = 4
}

public enum OrganizationStatus
{
    Active = 0,
    Pilot = 1,
    Trial = 2,
    Inactive = 3
}