using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.UserProfile.UserProfiles;

public class UserProfileDto : FullAuditedEntityDto<Guid>
{
    public string ExternalUserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public SystemRoleDto SystemRole { get; set; }
    public StakeholderTypeDto StakeholderType { get; set; }
    public string? PrimaryRole { get; set; }
    public Guid OrganizationId { get; set; }
    public string? AssignedClients { get; set; }
    public string? JobTitle { get; set; }
    public string? Phone { get; set; }
    public string? Country { get; set; }
    public string? Location { get; set; }
    public string? Department { get; set; }
    public string? Bio { get; set; }
    public string? Skills { get; set; }
    public bool ProfileCompleted { get; set; }
    public Guid? TenantId { get; set; }
}
