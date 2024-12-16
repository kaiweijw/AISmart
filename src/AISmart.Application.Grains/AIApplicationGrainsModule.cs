using System.Reflection;
//using AElf.Indexing.Elasticsearch;
using AISmart.Application.Grains.Command;
using AISmart.Application.Grains.CommandHandler;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Modularity;
using MediatR;
using Nest;

namespace AISmart.Application.Grains;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AbpEventBusModule),
    typeof(AISmartApplicationContractsModule)
    //typeof(AElfIndexingElasticsearchModule)
)]
public class AIApplicationGrainsModule : AbpModule
 
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        //context.Services.AddMediatR(Assembly.GetExecutingAssembly());
        context.Services.AddSingleton<ILocalEventBus, LocalEventBus>();
        context.Services.AddTransient<CreateEventCommandHandler>();
        context.Services.AddTransient<ILocalEventHandler<CreateEventComamand>, CreateEventCommandHandler>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AIApplicationGrainsModule>(); });
       // ConfigureElasticsearch(context);

    }
    
    private static void ConfigureElasticsearch(
        ServiceConfigurationContext context)
    {
        context.Services.AddSingleton<IElasticClient>(sp =>
        {
            var uris = new string[] {"http://127.0.0.1:9200"};
            if (uris == null || uris.Length == 0)
            {
                throw new ArgumentNullException("ElasticUris:Uris", "Elasticsearch URIs cannot be null or empty.");
            }

            var settings = new ConnectionSettings(new StaticConnectionPool(uris.Select(uri => new Uri(uri)).ToArray()));

            return new ElasticClient(settings);
        });
    
    } 
}