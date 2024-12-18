using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Orleans.Configuration;
using Orleans.EventSourcing;

namespace AISmart.LogStorage.MongoDB;

public static class MongoDbStorageSiloBuilderExtensions
{
    public static ISiloBuilder AddMongoDbStorageBasedLogConsistencyProvider(this ISiloBuilder builder, string name = "LogStorage")
    {
        builder.ConfigureServices(services =>
        {
            services.AddMongoDbStorageBasedLogConsistencyProvider(name);
        });

        return builder;
    }
    
    internal static void AddMongoDbStorageBasedLogConsistencyProvider(this IServiceCollection services, string name)
    {
        services.AddSingleton<ILogConsistencyProtocolServices, MongoDbLogConsistencyProtocolServices>();
        services.AddSingleton<MongoDbLogConsistencyOptions>();
        services.AddOptions<MongoDbLogConsistencyOptions>()
            .Configure(options =>
            {
                var configuration = services.GetConfiguration().GetSection("Orleans");
                options.MongoDBClient = configuration.GetValue<string>("MongoDBClient");
                options.DataBase = configuration.GetValue<string>("DataBase");
            });
        var configuration = services.GetConfiguration().GetSection("Orleans");
        var options = new MongoDbLogConsistencyOptions
        {
            MongoDBClient = configuration.GetValue<string>("MongoDBClient"),
            DataBase = configuration.GetValue<string>("DataBase")
        };
        services.ConfigureNamedOptionForLogging<MongoDbLogConsistencyOptions>(name)
            .AddKeyedSingleton(name, (sp, _) => MongoDbLogConsistencyProviderFactory.Create(options))
            .TryAddSingleton(sp => sp.GetKeyedService<ILogViewAdaptorFactory>("Default"));
    }
}