using AISmart.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace AISmart.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AISmartController : AbpControllerBase
{
    protected AISmartController()
    {
        LocalizationResource = typeof(AISmartResource);
    }
}
