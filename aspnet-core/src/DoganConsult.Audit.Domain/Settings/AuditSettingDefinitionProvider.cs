using Volo.Abp.Settings;

namespace DoganConsult.Audit.Settings;

public class AuditSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AuditSettings.MySetting1));
    }
}
