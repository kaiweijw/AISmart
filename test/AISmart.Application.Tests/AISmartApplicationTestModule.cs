using System;
using AISmart.CQRS.Handler;
using AISmart.Options;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;

namespace AISmart;

[DependsOn(
    typeof(AISmartApplicationModule),
    typeof(AbpEventBusModule),
    typeof(AISmartOrleansTestBaseModule),
    typeof(AISmartDomainTestModule)
)]
public class AISmartApplicationTestModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        base.ConfigureServices(context);
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AISmartApplicationModule>(); });
        var configuration = context.Services.GetConfiguration();
        Configure<ChatConfigOptions>(configuration.GetSection("Chat"));   
        context.Services.AddSingleton<IElasticClient>(provider =>
        {
            var settings =new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .DefaultIndex("cqrs");
            return new ElasticClient(settings);
        });
        context.Services.AddMediatR(typeof(GetStateQueryHandler).Assembly);
        

    }
}