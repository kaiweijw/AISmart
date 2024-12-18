using Microsoft.Extensions.DependencyInjection;
using Orleans.EventSourcing;

namespace AISmart.LogStorage.MongoDB;

public static class MongoDbLogConsistencyProviderFactory
{
    public static ILogViewAdaptorFactory Create(MongoDbLogConsistencyOptions options)
    {
        return new MongoDbLogConsistencyProvider(options);
    }
}