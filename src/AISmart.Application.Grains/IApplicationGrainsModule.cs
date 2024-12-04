using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AISmart.Application.Grains;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AISmartApplicationContractsModule)
)]
public class AIApplicationGrainsModule : AbpModule
 
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AIApplicationGrainsModule>(); });
        
    }
}