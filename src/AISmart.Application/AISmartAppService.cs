using System;
using System.Collections.Generic;
using System.Text;
using AISmart.Localization;
using Volo.Abp.Application.Services;

namespace AISmart;

/* Inherit your application services from this class.
 */
public abstract class AISmartAppService : ApplicationService
{
    protected AISmartAppService()
    {
        LocalizationResource = typeof(AISmartResource);
    }
}
