using System.Collections.Generic;
using System.Threading.Tasks;
using DoganConsult.Workspace.Features;
using DoganConsult.Workspace.UI;
using Volo.Abp.DependencyInjection;

namespace DoganConsult.Web.Blazor.Modules.Sbg;

/// <summary>
/// Sample UI contributor for SBG module demonstrating the plugin architecture.
/// This contributor will only be active when DG.Modules.SBG feature is enabled.
/// </summary>
public class SbgSampleUiContributor : IDgModuleUiContributor, ITransientDependency
{
    public string RequiredFeature => DgFeatures.Modules.Sbg;

    public Task<IEnumerable<MenuContribution>> GetMenuContributionsAsync()
    {
        var contributions = new List<MenuContribution>
        {
            new MenuContribution
            {
                Name = "DoganConsult.Sbg.Sample",
                DisplayName = "SBG Sample",
                Url = "/sbg/sample",
                Icon = "fas fa-building",
                Order = 100,
                RequiredFeature = DgFeatures.Modules.Sbg,
                Items = new List<MenuContribution>
                {
                    new MenuContribution
                    {
                        Name = "DoganConsult.Sbg.Sample.Overview",
                        DisplayName = "Overview",
                        Url = "/sbg/sample",
                        Icon = "fas fa-chart-line",
                        Order = 1
                    },
                    new MenuContribution
                    {
                        Name = "DoganConsult.Sbg.Sample.Procurement",
                        DisplayName = "Procurement",
                        Url = "/sbg/sample/procurement",
                        Icon = "fas fa-shopping-cart",
                        Order = 2,
                        RequiredFeature = DgFeatures.SubFeatures.Sbg.Procurement
                    },
                    new MenuContribution
                    {
                        Name = "DoganConsult.Sbg.Sample.Contracts",
                        DisplayName = "Contracts",
                        Url = "/sbg/sample/contracts",
                        Icon = "fas fa-file-contract",
                        Order = 3,
                        RequiredFeature = DgFeatures.SubFeatures.Sbg.Contracts
                    }
                }
            }
        };

        return Task.FromResult<IEnumerable<MenuContribution>>(contributions);
    }

    public Task<IEnumerable<DashboardWidgetContribution>> GetDashboardWidgetsAsync()
    {
        var widgets = new List<DashboardWidgetContribution>
        {
            new DashboardWidgetContribution
            {
                Name = "SbgSample.QuickStats",
                DisplayName = "SBG Quick Stats",
                ComponentType = "DoganConsult.Web.Blazor.Modules.Sbg.SbgQuickStatsWidget, DoganConsult.Web.Blazor",
                Order = 10,
                ColumnSpan = 4,
                RequiredFeature = DgFeatures.Modules.Sbg
            },
            new DashboardWidgetContribution
            {
                Name = "SbgSample.RecentActivity",
                DisplayName = "SBG Recent Activity",
                ComponentType = "DoganConsult.Web.Blazor.Modules.Sbg.SbgRecentActivityWidget, DoganConsult.Web.Blazor",
                Order = 11,
                ColumnSpan = 8,
                RequiredFeature = DgFeatures.Modules.Sbg
            }
        };

        return Task.FromResult<IEnumerable<DashboardWidgetContribution>>(widgets);
    }
}

