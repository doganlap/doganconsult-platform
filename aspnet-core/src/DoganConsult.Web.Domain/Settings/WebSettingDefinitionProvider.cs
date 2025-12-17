using Volo.Abp.Settings;

namespace DoganConsult.Web.Settings;

public class WebSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WebSettings.MySetting1));
    }
}
