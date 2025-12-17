using System.Threading.Tasks;
using DoganConsult.Web.Localization;
using DoganConsult.Web.MultiTenancy;
using Volo.Abp.Identity.Blazor;
using Volo.Abp.SettingManagement.Blazor.Menus;
using Volo.Abp.TenantManagement.Blazor.Navigation;
using Volo.Abp.UI.Navigation;

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

    private Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<WebResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                WebMenus.Home,
                l["Menu:Home"],
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
                l["Menu:Organizations"],
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
                l["Menu:Workspaces"],
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
                l["Menu:Documents"],
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
                l["Menu:UserProfiles"],
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
                l["Menu:AIChat"],
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
                l["Menu:AuditLogs"],
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
                l["Menu:Approvals"],
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

        return Task.CompletedTask;
    }
}
