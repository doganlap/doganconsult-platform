using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Organization.Localization;
using DoganConsult.Organization.Domain.Shared.Permissions;
using DoganConsult.Web.Localization;
using DoganConsult.Web.MultiTenancy;
using DoganConsult.Web.Blazor.Services;
using DoganConsult.Workspace.UI;
using Volo.Abp.Features;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor;
using Volo.Abp.TenantManagement.Blazor.Navigation;

namespace DoganConsult.Web.Blazor.Menus;

public class WebMenuContributor : IMenuContributor
{
    private readonly DgModuleUiService? _moduleUiService;
    private readonly IFeatureChecker? _featureChecker;

    public WebMenuContributor(DgModuleUiService? moduleUiService = null, IFeatureChecker? featureChecker = null)
    {
        _moduleUiService = moduleUiService;
        _featureChecker = featureChecker;
    }

    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var l = context.GetLocalizer<OrganizationResource>();

        // Clear existing menu items to replace with the new IA
        context.Menu.Items.Clear();

        // HOME (DASHBOARD)
        context.Menu.AddItem(new ApplicationMenuItem(
                "Platform.Home",
                l["Menu:Home"],
                "/",
                icon: "fas fa-home"
            )
        );

        // PLATFORM
        context.Menu.AddItem(new ApplicationMenuItem(
                name: "Platform",
                displayName: l["Menu:Platform"],
                icon: "fas fa-layer-group"
            )
            .AddItem(new ApplicationMenuItem(
                    name: "Platform.WorkCenter",
                    displayName: l["Menu:WorkCenter"],
                    url: "/work-center",
                    icon: "fas fa-inbox"
                ).RequirePermissions(OrganizationPermissions.Platform.WorkCenter))
            .AddItem(new ApplicationMenuItem(
                    name: "Platform.Approvals",
                    displayName: l["Menu:Approvals"],
                    url: "/approvals",
                    icon: "fas fa-check-circle"
                ).RequirePermissions(OrganizationPermissions.Platform.Approvals))
        );

        // ORGANIZATION
        context.Menu.AddItem(new ApplicationMenuItem(
                name: "Organization",
                displayName: l["Menu:Organization"],
                icon: "fas fa-sitemap"
            )
            .AddItem(new ApplicationMenuItem(
                    name: "Organization.Organizations",
                    displayName: l["Menu:Organizations"],
                    url: "/organizations",
                    icon: "fas fa-building"
                ).RequirePermissions(OrganizationPermissions.Org.Organizations))
            .AddItem(new ApplicationMenuItem(
                    name: "Organization.Workspaces",
                    displayName: l["Menu:Workspaces"],
                    url: "/workspaces",
                    icon: "fas fa-network-wired"
                ).RequirePermissions(OrganizationPermissions.Org.Workspaces))
            .AddItem(new ApplicationMenuItem(
                    name: "Organization.Profiles",
                    displayName: l["Menu:UserProfiles"],
                    url: "/user-profiles", // Changed from /profiles to match existing
                    icon: "fas fa-id-card"
                ).RequirePermissions(OrganizationPermissions.Org.Profiles))
        );

        // CONTENT
        context.Menu.AddItem(new ApplicationMenuItem(
                name: "Content",
                displayName: l["Menu:Content"],
                icon: "fas fa-folder-open"
            )
            .AddItem(new ApplicationMenuItem(
                    name: "Content.Documents",
                    displayName: l["Menu:Documents"],
                    url: "/documents",
                    icon: "fas fa-file-lines"
                ).RequirePermissions(OrganizationPermissions.Content.Documents))
            .AddItem(new ApplicationMenuItem(
                    name: "Content.AuditLogs",
                    displayName: l["Menu:AuditLogs"],
                    url: "/audit-logs",
                    icon: "fas fa-clipboard-list"
                ).RequirePermissions(OrganizationPermissions.Content.AuditLogs))
        );

        // ADMINISTRATION - Uses ABP's built-in administration menu
        // ABP modules will auto-register their own menu items under Administration
        
        // Load and add menu contributions from modules (if available)
        if (_moduleUiService != null)
        {
            try
            {
                var moduleContributions = await _moduleUiService.GetMenuContributionsAsync();
                foreach (var contribution in moduleContributions)
                {
                    var menuItem = new ApplicationMenuItem(
                        contribution.Name,
                        contribution.DisplayName,
                        contribution.Url,
                        icon: contribution.Icon,
                        order: contribution.Order
                    );

                    if (!string.IsNullOrEmpty(contribution.RequiredPermission))
                    {
                        menuItem.RequiredPermissionName = contribution.RequiredPermission;
                    }

                    if (!string.IsNullOrEmpty(contribution.RequiredFeature))
                    {
                        menuItem.Metadata.Add("RequiredFeature", contribution.RequiredFeature);
                    }

                    // Add sub-items recursively
                    foreach (var subItem in contribution.Items)
                    {
                        menuItem.AddItem(new ApplicationMenuItem(
                            subItem.Name,
                            subItem.DisplayName,
                            subItem.Url,
                            icon: subItem.Icon,
                            order: subItem.Order
                        ));
                    }

                    context.Menu.AddItem(menuItem);
                }
            }
            catch
            {
                // Module UI service not available, continue without it
            }
        }
    }
}
