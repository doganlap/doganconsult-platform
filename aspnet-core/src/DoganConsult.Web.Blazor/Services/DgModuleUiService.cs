using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Workspace.Features;
using DoganConsult.Workspace.UI;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Features;

namespace DoganConsult.Web.Blazor.Services;

/// <summary>
/// Service that discovers and aggregates UI contributions from all registered modules.
/// Automatically filters contributions based on feature flags.
/// </summary>
public class DgModuleUiService : ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IFeatureChecker _featureChecker;
    private List<IDgModuleUiContributor>? _contributors;

    public DgModuleUiService(IServiceProvider serviceProvider, IFeatureChecker featureChecker)
    {
        _serviceProvider = serviceProvider;
        _featureChecker = featureChecker;
    }

    /// <summary>
    /// Gets all menu contributions from enabled modules.
    /// </summary>
    public async Task<IEnumerable<MenuContribution>> GetMenuContributionsAsync()
    {
        var contributors = await GetEnabledContributorsAsync();
        var allContributions = new List<MenuContribution>();

        foreach (var contributor in contributors)
        {
            var contributions = await contributor.GetMenuContributionsAsync();
            allContributions.AddRange(contributions);
        }

        return allContributions.OrderBy(c => c.Order);
    }

    /// <summary>
    /// Gets all dashboard widget contributions from enabled modules.
    /// </summary>
    public async Task<IEnumerable<DashboardWidgetContribution>> GetDashboardWidgetsAsync()
    {
        var contributors = await GetEnabledContributorsAsync();
        var allWidgets = new List<DashboardWidgetContribution>();

        foreach (var contributor in contributors)
        {
            var widgets = await contributor.GetDashboardWidgetsAsync();
            allWidgets.AddRange(widgets);
        }

        return allWidgets.OrderBy(w => w.Order);
    }

    private async Task<List<IDgModuleUiContributor>> GetEnabledContributorsAsync()
    {
        if (_contributors != null)
            return _contributors;

        _contributors = new List<IDgModuleUiContributor>();
        var allContributors = _serviceProvider.GetServices<IDgModuleUiContributor>();

        foreach (var contributor in allContributors)
        {
            if (string.IsNullOrEmpty(contributor.RequiredFeature) ||
                await _featureChecker.IsEnabledAsync(contributor.RequiredFeature))
            {
                _contributors.Add(contributor);
            }
        }

        return _contributors;
    }
}

