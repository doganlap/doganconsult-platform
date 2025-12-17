using Volo.Abp.Settings;

namespace DoganConsult.Document.Settings;

public class DocumentSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(DocumentSettings.MySetting1));
    }
}
