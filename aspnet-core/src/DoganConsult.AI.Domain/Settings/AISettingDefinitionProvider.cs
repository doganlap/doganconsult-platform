using Volo.Abp.Settings;

namespace DoganConsult.AI.Settings;

public class AISettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AISettings.MySetting1));
    }
}
