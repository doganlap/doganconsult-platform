using System;
using System.ComponentModel.DataAnnotations;

namespace DoganConsult.Workspace.Workspaces;

public class CreateUpdateWorkspaceDto
{
    [Required]
    [StringLength(50)]
    public string Code { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid OrganizationId { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string Status { get; set; } = "active";

    [StringLength(500)]
    public string? Settings { get; set; }

    [StringLength(1000)]
    public string? Members { get; set; }

    [StringLength(500)]
    public string? WorkspaceOwner { get; set; }

    [StringLength(500)]
    public string? Permissions { get; set; }
}
