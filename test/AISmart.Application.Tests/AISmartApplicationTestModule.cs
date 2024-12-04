using AISmart.Dapr;
using AISmart.Mock;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AISmart;

[DependsOn(
    typeof(AISmartApplicationModule),
    typeof(AISmartOrleansTestBaseModule),
    typeof(AISmartDomainTestModule)
)]
public class AISmartApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        context.Services.AddSingleton<IDaprProvider , MockDaprProvider>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AISmartApplicationModule>(); });
           
    }
}
