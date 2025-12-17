using Volo.Abp.Settings;

namespace DoganConsult.Workspace.Settings;

public class WorkspaceSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(WorkspaceSettings.MySetting1));
    }
}
