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
    public Guid? TenantId { get; set; }
}
