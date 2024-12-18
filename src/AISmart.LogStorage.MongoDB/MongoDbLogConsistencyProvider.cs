using Orleans.Configuration;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;
using Orleans.Storage;

namespace AISmart.LogStorage.MongoDB;

public class MongoDbLogConsistencyProvider : ILogViewAdaptorFactory
{
    private readonly MongoDbLogConsistencyOptions _options;

    public bool UsesStorageProvider => false;

    public MongoDbLogConsistencyProvider(MongoDbLogConsistencyOptions options)
    {
        _options = options;
    }

    public ILogViewAdaptor<TLogView, TLogEntry> MakeLogViewAdaptor<TLogView, TLogEntry>(
        ILogViewAdaptorHost<TLogView, TLogEntry> hostGrain, TLogView initialState,
        string grainTypeName, IGrainStorage grainStorage, ILogConsistencyProtocolServices services)
        where TLogView : class, new()
        where TLogEntry : class
    {
        return new MongoDbStorageAdaptor<TLogView, TLogEntry>(hostGrain, initialState, services, _options);
    }
}