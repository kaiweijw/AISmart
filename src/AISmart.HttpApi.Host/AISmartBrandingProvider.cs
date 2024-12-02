using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace AISmart;

[Dependency(ReplaceServices = true)]
public class AISmartBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "AISmart";
}
