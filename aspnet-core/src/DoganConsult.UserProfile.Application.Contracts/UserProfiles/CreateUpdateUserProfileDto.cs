using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.UserProfile.UserProfiles;

public class CreateUpdateUserProfileDto
{
    [Required]
    [StringLength(200)]
    public string ExternalUserId { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    public SystemRoleDto SystemRole { get; set; } = SystemRoleDto.User;

    [Required]
    public StakeholderTypeDto StakeholderType { get; set; } = StakeholderTypeDto.DemoGuest;

    [StringLength(200)]
    public string? PrimaryRole { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

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

    [StringLength(1000)]
    public string? Skills { get; set; }

    [StringLength(200)]
    public string? Title { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }

    public Guid? ManagerId { get; set; }

    public DateTime? StartDate { get; set; }

    [StringLength(50)]
    public string? Availability { get; set; }

    public bool ProfileCompleted { get; set; } = false;
}
