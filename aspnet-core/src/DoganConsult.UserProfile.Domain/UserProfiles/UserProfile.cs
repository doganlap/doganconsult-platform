using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.MultiTenancy;

namespace DoganConsult.UserProfile.UserProfiles;

public class UserProfile : FullAuditedAggregateRoot<Guid>, IMultiTenant
{
    [Required]
    [StringLength(200)]
    public string ExternalUserId { get; set; } = string.Empty; // Entra ID ObjectId or sub

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    public SystemRole SystemRole { get; set; } = SystemRole.User;

    public StakeholderType StakeholderType { get; set; } = StakeholderType.DemoGuest;

    [StringLength(200)]
    public string? PrimaryRole { get; set; }

    public Guid OrganizationId { get; set; }

    /// <summary>
    /// Comma-separated list of assigned client organization codes
    /// </summary>
    [StringLength(1000)]
    public string? AssignedClients { get; set; }

    [StringLength(200)]
    public string? JobTitle { get; set; }

    [StringLength(50)]
    public string? Phone { get; set; }

    [StringLength(100)]
    public string? Country { get; set; }

    [StringLength(100)]
    public string? Location { get; set; }

    [StringLength(200)]
    public string? Department { get; set; }

    [StringLength(2000)]
    public string? Bio { get; set; }

    /// <summary>
    /// Comma-separated list of skills
    /// </summary>
    [StringLength(1000)]
    public string? Skills { get; set; }

    public bool ProfileCompleted { get; set; } = false;

    public Guid? TenantId { get; set; }

    protected UserProfile()
    {
    }

    public UserProfile(
        Guid id,
        string externalUserId,
        string email,
        string fullName,
        Guid organizationId,
        Guid? tenantId = null)
        : base(id)
    {
        ExternalUserId = externalUserId;
        Email = email;
        FullName = fullName;
        OrganizationId = organizationId;
        TenantId = tenantId;
    }
}
