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
                "Organizations",
                "/organizations",
                icon: "fas fa-building",
                order: 1
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
