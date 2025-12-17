using Volo.Abp.Settings;

namespace DoganConsult.UserProfile.Settings;

public class UserProfileSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(UserProfileSettings.MySetting1));
    }
}
