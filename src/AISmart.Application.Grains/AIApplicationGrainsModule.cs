using System.Reflection;
using AElf.Indexing.Elasticsearch.Options;
using AISmart.Application.Grains.Command;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.AutoMapper;
using Volo.Abp.EventBus;
using Volo.Abp.Modularity;
using MediatR;
using Nest;
using Volo.Abp;
namespace AISmart.Application.Grains;

[DependsOn(
    typeof(AbpAutoMapperModule),
    typeof(AbpEventBusModule),
    typeof(AISmartApplicationContractsModule)
   //,typeof(AISmartApplicationModule)
)]
public class AIApplicationGrainsModule : AbpModule
 
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddMediatR(Assembly.GetExecutingAssembly());
        //context.Services.AddSingleton<ILocalEventBus, LocalEventBus>();
        //context.Services.AddTransient<ILocalEventHandler<CreateEventComamand>, CreateEventCommandHandler>();
        Configure<AbpAutoMapperOptions>(options => { options.AddMaps<AIApplicationGrainsModule>(); });
        
        context.Services.Configure<EsEndpointOption>(options =>
        {
            options.Uris = new List<string> { "http://127.0.0.1:9200" };
        });

        context.Services.Configure<IndexSettingOptions>(options =>
        {
            options.NumberOfReplicas = 1;
            options.NumberOfShards = 1;
            options.Refresh = Refresh.True;
            options.IndexPrefix = "aismart";
        });
        context.Services.AddMediatR(typeof(SaveStateCommandHandler).Assembly);
        context.Services.AddTransient<SaveStateCommandHandler>();

        ConfigureElasticsearch(context);

    }
    public override void OnApplicationShutdown(ApplicationShutdownContext context)
    {
        // var elasticIndexService = context.ServiceProvider.GetRequiredService<IElasticIndexService>();
        // var modules = context.ServiceProvider.GetRequiredService<IOptionsSnapshot<IndexCreateOption>>().Value.Modules;
        //
        // modules.ForEach(m =>
        // {
        //     var types = GetTypesAssignableFrom<IIndexBuild>(m.Assembly);
        //     foreach (var t in types)
        //     {
        //         AsyncHelper.RunSync(async () =>
        //             await elasticIndexService.DeleteIndexAsync("aismart." + t.Name.ToLower()));
        //     }
        // });
    }
    
    private List<Type> GetTypesAssignableFrom<T>(Assembly assembly)
    {
        var compareType = typeof(T);
        return assembly.DefinedTypes
            .Where(type => compareType.IsAssignableFrom(type) && !compareType.IsAssignableFrom(type.BaseType) &&
                           !type.IsAbstract && type.IsClass && compareType != type)
            .Cast<Type>().ToList();
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