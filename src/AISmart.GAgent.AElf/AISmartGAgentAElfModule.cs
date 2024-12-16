using System.Reflection;
using AISmart.Application.Grains;
using AISmart.Options;
using AISmart.Provider;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.Modularity;

namespace AISmart;

[DependsOn(
    typeof(AISmartApplicationContractsModule),
    typeof(AIApplicationGrainsModule)
    )]
public class AISmartGAgentAElfModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AISmartGAgentAElfModule>();
        });
        var configuration = context.Services.GetConfiguration();
        Configure<ChainOptions>(configuration.GetSection("Chain"));   
    }
}
