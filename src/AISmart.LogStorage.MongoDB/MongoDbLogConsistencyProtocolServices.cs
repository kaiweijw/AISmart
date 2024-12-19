using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Serialization;

namespace AISmart.LogStorage.MongoDB;

public class MongoDbLogConsistencyProtocolServices : ILogConsistencyProtocolServices
{
    private readonly ILogger? _logger;
    private readonly DeepCopier _deepCopier;
    private readonly IGrainContext _grainContext; // links to the grain that owns this service object

    public MongoDbLogConsistencyProtocolServices(
        IGrainContext grainContext,
        ILoggerFactory loggerFactory,
        DeepCopier deepCopier,
        ILocalSiloDetails siloDetails)
    {
        _grainContext = grainContext;
        _logger = loggerFactory.CreateLogger<MongoDbLogConsistencyProtocolServices>();
        _deepCopier = deepCopier;
        MyClusterId = siloDetails.ClusterId;
    }

    public GrainId GrainId => _grainContext.GrainId;

    public string MyClusterId { get; }

    public T DeepCopy<T>(T value) => this._deepCopier.Copy(value);

    public void ProtocolError(string msg, bool throwexception)
    {
        _logger?.LogError(
            (int)(throwexception
                ? ErrorCode.LogConsistency_ProtocolFatalError
                : ErrorCode.LogConsistency_ProtocolError),
            "{GrainId} Protocol Error: {Message}",
            _grainContext.GrainId,
            msg);

        if (!throwexception)
        {
            return;
        }

        throw new OrleansException(string.Format("{0} (grain={1}, cluster={2})", msg, _grainContext.GrainId,
            MyClusterId));
    }

    public void CaughtException(string where, Exception e)
    {
        _logger?.LogError(
            (int)ErrorCode.LogConsistency_CaughtException,
            e,
            "{GrainId} exception caught at {Location}",
            _grainContext.GrainId,
            where);
    }

    public void CaughtUserCodeException(string callback, string where, Exception e)
    {
        _logger?.LogWarning(
            (int)ErrorCode.LogConsistency_UserCodeException,
            e,
            "{GrainId} exception caught in user code for {Callback}, called from {Location}",
            _grainContext.GrainId,
            callback,
            where);
    }

    public void Log(LogLevel level, string format, params object[] args)
    {
        if (_logger != null && _logger.IsEnabled(level))
        {
            var msg = $"{_grainContext.GrainId} {string.Format(format, args)}";
            _logger.Log(level, 0, msg, null, (m, exc) => $"{m}");
        }
    }
}