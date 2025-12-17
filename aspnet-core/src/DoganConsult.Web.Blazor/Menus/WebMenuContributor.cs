using System.Linq;
using System.Threading.Tasks;
using DoganConsult.Web.Localization;
using DoganConsult.Web.MultiTenancy;
using DoganConsult.Web.Blazor.Services;
using DoganConsult.Workspace.UI;
using Volo.Abp.Features;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

namespace DoganConsult.Web.Blazor.Menus;

public class WebMenuContributor : IMenuContributor
{
    private readonly DgModuleUiService _moduleUiService;
    private readonly IFeatureChecker _featureChecker;

    public WebMenuContributor(DgModuleUiService moduleUiService, IFeatureChecker featureChecker)
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
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<WebResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                WebMenus.Home,
                "Home",
                "/",
                icon: "fas fa-home",
                order: 0
            )
        );

        // Add Organization Management Menu
        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(
                WebMenus.Organizations,
                "Organizations",
                "/organizations",
                icon: "fas fa-building",
                order: 1
            )
        );

        // Add Workspaces Menu
        context.Menu.Items.Insert(
            2,
            new ApplicationMenuItem(
                "DoganConsult.Workspaces",
                "Workspaces",
                "/workspaces",
                icon: "fas fa-folder-open",
                order: 2
            )
        );

        // Add Documents Menu
        context.Menu.Items.Insert(
            3,
            new ApplicationMenuItem(
                "DoganConsult.Documents",
                "Documents",
                "/documents",
                icon: "fas fa-file-alt",
                order: 3
            )
        );

        // Add Users Menu
        context.Menu.Items.Insert(
            4,
            new ApplicationMenuItem(
                "DoganConsult.Users",
                "User Profiles",
                "/user-profiles",
                icon: "fas fa-users",
                order: 4
            )
        );

        // Add AI Assistant Menu
        context.Menu.Items.Insert(
            5,
            new ApplicationMenuItem(
                "DoganConsult.AI",
                "AI Chat",
                "/ai-chat",
                icon: "fas fa-robot",
                order: 5
            )
        );

        // Add Audit Logs Menu
        context.Menu.Items.Insert(
            6,
            new ApplicationMenuItem(
                "DoganConsult.AuditLogs",
                "Audit Logs",
                "/audit-logs",
                icon: "fas fa-history",
                order: 6
            )
        );

        // Add Approvals Menu
        context.Menu.Items.Insert(
            7,
            new ApplicationMenuItem(
                "DoganConsult.Approvals",
                "Approvals",
                "/approvals",
                icon: "fas fa-tasks",
                order: 7
            )
        );

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 2);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 3);
        administration.SetSubItemOrder(SettingManagementMenus.GroupName, 4);

        // Add Tenant Settings submenu under Administration
        administration.AddItem(new ApplicationMenuItem(
            "DoganConsult.TenantSettings",
            "Tenant Settings",
            icon: "fas fa-cog",
            order: 5
        ));

        var tenantSettings = administration.Items.FirstOrDefault(x => x.Name == "DoganConsult.TenantSettings");
        if (tenantSettings != null)
        {
            tenantSettings.AddItem(new ApplicationMenuItem(
                "DoganConsult.Branding",
                "Branding",
                "/admin/branding",
                icon: "fas fa-palette",
                order: 1
            ));
            tenantSettings.AddItem(new ApplicationMenuItem(
                "DoganConsult.Features",
                "Features",
                "/admin/features",
                icon: "fas fa-toggle-on",
                order: 2
            ));
        }

        // Load and add menu contributions from modules
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
                // Feature check is already done by DgModuleUiService, but we can add it as metadata
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
}
