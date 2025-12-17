namespace DoganConsult.Workspace.UI;

/// <summary>
/// Represents a dashboard widget contribution from a module.
/// </summary>
public class DashboardWidgetContribution
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ComponentType { get; set; } = string.Empty; // Full type name of Blazor component
    public int Order { get; set; }
    public int ColumnSpan { get; set; } = 1; // 1-12 for Bootstrap grid
    public string? RequiredPermission { get; set; }
    public string? RequiredFeature { get; set; }
}

