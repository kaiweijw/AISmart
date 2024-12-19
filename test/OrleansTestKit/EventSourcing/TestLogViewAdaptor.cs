using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.Common;

public class TestLogViewAdaptor<TLogView, TLogEntry> : 
    PrimaryBasedLogViewAdaptor<TLogView, TLogEntry, SubmissionEntry<TLogEntry>>
    where TLogView : class, new()
    where TLogEntry : class
{
    private readonly ILogViewAdaptorHost<TLogView, TLogEntry> _host;
    private readonly TLogView _initialState;

    public static readonly ICollection<ViewStateWrapper<TLogView>> ViewStateCollection =
        new List<ViewStateWrapper<TLogView>>();
    public static readonly ICollection<MongoDbEventLogWrapper<TLogEntry>> EventLogCollection =
        new List<MongoDbEventLogWrapper<TLogEntry>>();

    private TLogView _cachedView;
    private int _version;

    public TestLogViewAdaptor(ILogViewAdaptorHost<TLogView, TLogEntry> host, TLogView initialState,
        ILogConsistencyProtocolServices services)
        : base(host, initialState, services)
    {
        _host = host;
        _initialState = initialState;
    }

    protected override void InitializeConfirmedView(TLogView initialstate)
    {
        _cachedView = _initialState ?? new TLogView();
        _version = 0;
    }

    protected override TLogView LastConfirmedView()
    {
        return _cachedView!;
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
                LastPrimaryIssue.Record(new ReadFromPrimaryFailed { Exception = e }, Host, Services);
            }
            
            await LastPrimaryIssue.DelayBeforeRetry();
        }
    }

    protected override async Task<int> WriteAsync()
    {
        var latestView = await GetLatestViewAsync() ?? new ViewStateWrapper<TLogView>
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
        await SaveViewAsync(new ViewStateWrapper<TLogView>
        {
            Version = _version,
            State = _cachedView!
        });

        return updates.Length;
    }

    private Task SaveViewAsync(ViewStateWrapper<TLogView> viewState)
    {
        ViewStateCollection.Add(viewState);
        return Task.CompletedTask;
    }

    private Task SaveEventsAsync(IEnumerable<MongoDbEventLogWrapper<TLogEntry>> eventLogs)
    {
        foreach (var eventLog in eventLogs)
        {
            eventLog.Timestamp = DateTime.UtcNow;
            EventLogCollection.Add(eventLog);
        }
        return Task.CompletedTask;
    }
    
    private Task<ViewStateWrapper<TLogView>?> GetLatestViewAsync()
    {
        return Task.FromResult(ViewStateCollection
            .ToList()
            .OrderByDescending(v => v.Version)
            .FirstOrDefault());
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

public class ViewStateWrapper<T>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int Version { get; set; }
    public T State { get; set; }
}

public class MongoDbEventLogWrapper<T>
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

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