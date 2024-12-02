using AISmart.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace AISmart.Permissions;

public class AISmartPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AISmartPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(AISmartPermissions.MyPermission1, L("Permission:MyPermission1"));

    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AISmartResource>(name);
    }
}
