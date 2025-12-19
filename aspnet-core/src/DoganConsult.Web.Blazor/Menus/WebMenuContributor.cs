using System.Threading.Tasks;
using DoganConsult.Organization.Localization;
using DoganConsult.Organization.Domain.Shared.Permissions;
using Volo.Abp.UI.Navigation;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.TenantManagement.Blazor;

namespace DoganConsult.Web.Blazor.Menus;

public class WebMenuContributor : IMenuContributor
{
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
    }
}
