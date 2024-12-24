using Microsoft.Extensions.Logging;
using Orleans.Storage;

namespace AISmart.GAgent.Core;

public sealed class ContextStorageState
{
    public Dictionary<string, object?> Context { get; set; } = new();
}

public interface IContextStorageGrain : IGrainWithGuidKey
{
    Task AddContext(string key, object? value);
    Task AddContext(Dictionary<string, object?> context);
    Task<Dictionary<string, object?>> GetContext();
}

public class ContextStorageGrain : Grain<ContextStorageState>, IContextStorageGrain
{
    private readonly IPersistentState<ContextStorageState> _context;
    private readonly ILogger<ContextStorageGrain> _logger;

    private readonly IGrainStorage _grainStorage;

    public ContextStorageGrain([PersistentState("ContextStorage")] IPersistentState<ContextStorageState> context,
        ILogger<ContextStorageGrain> logger)
    {
        _context = context;
        _logger = logger;
    }

    public Task AddContext(string key, object? value)
    {
        _logger.LogInformation($"Adding context {key}: {value}, ContextStorageGrain's GrainId: {this.GetGrainId()}");

        _context.ReadStateAsync();

        if (_context.State.Context.IsNullOrEmpty())
        {
            _context.State.Context = [];
        }

        _context.State.Context[key] = value;
        _context.WriteStateAsync();
        return Task.CompletedTask;
    }

    public Task AddContext(Dictionary<string, object?> context)
    {
        _context.ReadStateAsync();

        if (_context.State.Context.IsNullOrEmpty())
        {
            _context.State.Context = context;
        }
        else
        {
            foreach (var keyPair in context)
            {
                _logger.LogInformation(
                    $"Adding context {keyPair.Key}: {keyPair.Value}, ContextStorageGrain's GrainId: {this.GetGrainId()}");
                _context.State.Context[keyPair.Key] = keyPair.Value;
            }
        }

        _context.WriteStateAsync();
        return Task.CompletedTask;
    }

    public Task<Dictionary<string, object?>> GetContext()
    {
        _context.ReadStateAsync();
        return Task.FromResult(_context.State.Context);
    }
}