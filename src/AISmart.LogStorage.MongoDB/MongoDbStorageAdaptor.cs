using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Orleans.EventSourcing;
using Orleans.EventSourcing.Common;

namespace AISmart.LogStorage.MongoDB;

public class MongoDbStorageAdaptor<TLogView, TLogEntry>
    : PrimaryBasedLogViewAdaptor<TLogView, TLogEntry, SubmissionEntry<TLogEntry>>
    where TLogView : class, new()
    where TLogEntry : class
{
    private readonly ILogViewAdaptorHost<TLogView, TLogEntry> _host;
    private readonly TLogView _initialState;
    private readonly ILogConsistencyProtocolServices _services;
    private readonly MongoDbLogConsistencyOptions _options;
    private readonly string _primaryCluster;

    private readonly IMongoCollection<MongoDbStateWrapper<TLogView>> _stateCollection;
    private readonly IMongoCollection<MongoDbEventWrapper<TLogEntry>> _eventCollection;

    private TLogView _cached;
    private int _version;

    public MongoDbStorageAdaptor(ILogViewAdaptorHost<TLogView, TLogEntry> host, TLogView initialState,
        ILogConsistencyProtocolServices services, MongoDbLogConsistencyOptions options)
        : base(host, initialState, services)
    {
        _host = host;
        _initialState = initialState;
        _services = services;
        _options = options;
        // TODO: Maybe abstraction this collection management
        _eventCollection = new MongoClient(options.MongoDBClient)
            .GetDatabase(options.DataBase)
            // TODO: Temporarily hard-coded collection name
            .GetCollection<MongoDbEventWrapper<TLogEntry>>("AISmartEvents");
        _stateCollection = new MongoClient(options.MongoDBClient)
            .GetDatabase(options.DataBase)
            // TODO: Temporarily hard-coded collection name
            .GetCollection<MongoDbStateWrapper<TLogView>>("AISmartStates");
    }

    protected override void InitializeConfirmedView(TLogView initialstate)
    {
        _cached = _initialState;
        _version = 0;
    }

    protected override TLogView LastConfirmedView()
    {
        return _cached ?? new TLogView();
    }

    protected override int GetConfirmedVersion() => _version;

    protected override bool SupportSubmissions => true;

    protected override async Task ReadAsync()
    {
        while (true)
        {
            try
            {
                var result = _stateCollection.Find(FilterDefinition<MongoDbStateWrapper<TLogView>>.Empty)
                    .SortByDescending(e => e.Version)
                    .FirstOrDefault();

                if (result != null)
                {
                    _version = result.Version;
                    _cached = result.State;
                }

                Services.Log(LogLevel.Debug, "read success v{0}", _version);

                LastPrimaryIssue.Resolve(_host, Services);

                break;
            }
            catch (Exception e)
            {
                // unwrap inner exception that was forwarded - helpful for debugging
                if ((e as ProtocolTransportException)?.InnerException != null)
                {
                    e = ((ProtocolTransportException)e).InnerException!;
                }
                LastPrimaryIssue.Record(new ReadFromPrimaryFailed() { Exception = e }, Host, Services);
            }
            
            Services.Log(LogLevel.Debug, "read failed {0}", LastPrimaryIssue);

            await LastPrimaryIssue.DelayBeforeRetry();
        }
    }

    protected override async Task<int> WriteAsync()
    {
        var currentView = await _stateCollection.Find(FilterDefinition<MongoDbStateWrapper<TLogView>>.Empty)
            .SortByDescending(s => s.Version)
            .FirstOrDefaultAsync();
        var currentVersion = currentView.Version;

        var newView = new MongoDbStateWrapper<TLogView>
        {
            Version = currentVersion + 1,
            State = currentView.State
        };
        await _stateCollection.InsertOneAsync(newView);

        var updates = GetCurrentBatchOfUpdates()
            .Select(e => e.Entry)
            .Select(e => new MongoDbEventWrapper<TLogEntry>
            {
                Version = currentVersion,
                Event = e
            }).ToList();

        await _eventCollection.InsertManyAsync(updates);

        foreach (var update in updates)
        {
            _host.UpdateView(_cached, update.Event);
        }

        _cached = currentView.State;

        Services.Log(LogLevel.Debug, "write success v{0}", _version);

        return updates.Count;
    }

    protected override SubmissionEntry<TLogEntry> MakeSubmissionEntry(TLogEntry entry)
    {
        return new SubmissionEntry<TLogEntry> { Entry = entry };
    }

    protected override Task<ILogConsistencyProtocolMessage> OnMessageReceived(ILogConsistencyProtocolMessage payload)
    {
        var request = (ReadRequest)payload;

        var response = new ReadResponse<TLogView>() { Version = _version };

        if (_version > request.KnownVersion)
        {
            response.Value = _cached;
        }

        return Task.FromResult<ILogConsistencyProtocolMessage>(response);
    }
}

[Serializable]
[GenerateSerializer]
internal sealed class ReadRequest : ILogConsistencyProtocolMessage
{
    [Id(0)] public int KnownVersion { get; set; }
}

[Serializable]
[GenerateSerializer]
internal sealed class ReadResponse<TViewType> : ILogConsistencyProtocolMessage
{
    [Id(0)] public int Version { get; set; }

    [Id(1)] public TViewType Value { get; set; }
}

public class MongoDbStateWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int Version { get; set; }
    public T State { get; set; }
}

public class MongoDbEventWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int Version { get; set; }
    public T Event { get; set; }
}

[Serializable]
[GenerateSerializer]
public sealed class ReadFromPrimaryFailed : PrimaryOperationFailed
{
    /// <inheritdoc/>
    public override string ToString()
    {
        return $"read from primary failed: caught {Exception.GetType().Name}: {Exception.Message}";
    }
}