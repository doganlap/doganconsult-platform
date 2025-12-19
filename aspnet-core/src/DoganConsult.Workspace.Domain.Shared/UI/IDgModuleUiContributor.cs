using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoganConsult.Workspace.UI;

/// <summary>
/// Interface for modules to contribute UI elements (menu items, dashboard widgets) to the platform shell.
/// All contributions are automatically feature-gated based on the RequiredFeature property.
/// </summary>
public interface IDgModuleUiContributor
{
    /// <summary>
    /// The feature name that must be enabled for this contributor's UI elements to be shown.
    /// Use DgFeatures.Modules.* constants.
    /// </summary>
    string RequiredFeature { get; }

    /// <summary>
    /// Gets menu contributions for this module.
    /// Only called if RequiredFeature is enabled.
    /// </summary>
    Task<IEnumerable<MenuContribution>> GetMenuContributionsAsync();

    /// <summary>
    /// Gets dashboard widget contributions for this module.
    /// Only called if RequiredFeature is enabled.
    /// </summary>
    Task<IEnumerable<DashboardWidgetContribution>> GetDashboardWidgetsAsync();
}

