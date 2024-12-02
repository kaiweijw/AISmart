using Volo.Abp.Settings;

namespace AISmart.Settings;

public class AISmartSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AISmartSettings.MySetting1));
    }
}
