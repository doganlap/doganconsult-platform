using System;
using Volo.Abp.Application.Dtos;

namespace DoganConsult.Workspace.Workspaces;

public class WorkspaceDto : FullAuditedEntityDto<Guid>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public Guid OrganizationId { get; set; }
    public string? Description { get; set; }
    public string Status { get; set; } = "active";
    public string? Settings { get; set; }
    public string? Members { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? WorkspaceOwner { get; set; }
    public string? Permissions { get; set; }
    public Guid? TenantId { get; set; }
}
