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
    private readonly TLogView? _initialState;

    private IMongoCollection<MongoDbViewStateWrapper<TLogView>> _viewStateCollection;
    private IMongoCollection<MongoDbEventLogWrapper<TLogEntry>> _eventLogCollection;

    private TLogView _cachedView;
    private int _version;

    public MongoDbStorageAdaptor(ILogViewAdaptorHost<TLogView, TLogEntry> host, TLogView initialState,
        ILogConsistencyProtocolServices services, MongoDbLogConsistencyOptions options)
        : base(host, initialState, services)
    {
        _host = host;
        _initialState = initialState;

        InitializeMongoCollections(options);
    }

    private void InitializeMongoCollections(MongoDbLogConsistencyOptions options)
    {
        var client = new MongoClient(options.MongoDBClient);
        var database = client.GetDatabase(options.DataBase);

        if (!database.ListCollectionNames().ToList().Contains(MongoDbStorageConstants.StateCollectionName))
        {
            database.CreateCollection(MongoDbStorageConstants.StateCollectionName);
        }

        if (!database.ListCollectionNames().ToList().Contains(MongoDbStorageConstants.EventCollectionName))
        {
            database.CreateCollection(MongoDbStorageConstants.EventCollectionName);
        }

        _viewStateCollection =
            database.GetCollection<MongoDbViewStateWrapper<TLogView>>(MongoDbStorageConstants.StateCollectionName);
        _eventLogCollection =
            database.GetCollection<MongoDbEventLogWrapper<TLogEntry>>(MongoDbStorageConstants.EventCollectionName);
    }

    protected override void InitializeConfirmedView(TLogView initialstate)
    {
        _cachedView = _initialState ?? new TLogView();
        _version = 0;
    }

    protected override TLogView LastConfirmedView()
    {
        return _cachedView;
    }

    protected override int GetConfirmedVersion() => _version;

    protected override bool SupportSubmissions => true;

    protected override async Task ReadAsync()
    {
        while (true)
        {
            try
            {
                var result = await GetLatestViewAsync();

                if (result != null)
                {
                    _version = result.Version;
                    _cachedView = result.State;
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
        var latestView = await GetLatestViewAsync() ?? new MongoDbViewStateWrapper<TLogView>
        {
            Version = _version,
            State = _cachedView
        };

        var latestVersion = latestView.Version;
        if (latestVersion != _version)
        {
            // Return if version not match.
            return 0;
        }

        var updates = GetCurrentBatchOfUpdates();
        var logsToUpdate = updates
            .Select(e => e.Entry)
            .Select(e => new MongoDbEventLogWrapper<TLogEntry>
            {
                Version = latestVersion + 1,
                Event = e
            }).ToList();
        
        // Save logs to database.
        await SaveEventsAsync(logsToUpdate);

        // Update grain's view.
        foreach (var eventLog in logsToUpdate)
        {
            _version++;
            _host.UpdateView(_cachedView!, eventLog.Event);
        }

        // Update the view to database.
        await SaveViewAsync(new MongoDbViewStateWrapper<TLogView>
        {
            Version = _version,
            State = _cachedView!
        });

        Services.Log(LogLevel.Debug, "write success v{0}", _version);

        return updates.Length;
    }

    private async Task SaveViewAsync(MongoDbViewStateWrapper<TLogView> viewState)
    {
        await _viewStateCollection.ReplaceOneAsync(
            filter: Builders<MongoDbViewStateWrapper<TLogView>>.Filter.Eq(v => v.Id, viewState.Id),
            replacement: viewState,
            options: new ReplaceOptions { IsUpsert = true });
    }

    private async Task SaveEventsAsync(IEnumerable<MongoDbEventLogWrapper<TLogEntry>> eventLogs)
    {
        await _eventLogCollection.InsertManyAsync(eventLogs);
    }
    
    private async Task<MongoDbViewStateWrapper<TLogView>?> GetLatestViewAsync()
    {
        return await _viewStateCollection
            .Find(FilterDefinition<MongoDbViewStateWrapper<TLogView>>.Empty)
            .SortByDescending(v => v.Version)
            .FirstOrDefaultAsync();
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
            response.Value = _cachedView;
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

public class MongoDbViewStateWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int Version { get; set; }
    public T State { get; set; }
}

public class MongoDbEventLogWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();

    public int Version { get; set; }
    public T Event { get; set; }
    public DateTime Timestamp { get; set; }
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