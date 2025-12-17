using System.Collections.Generic;

namespace DoganConsult.Workspace.UI;

/// <summary>
/// Represents a menu item contribution from a module.
/// </summary>
public class MenuContribution
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int Order { get; set; }
    public string? RequiredPermission { get; set; }
    public string? RequiredFeature { get; set; }
    public List<MenuContribution> Items { get; set; } = new();
}

