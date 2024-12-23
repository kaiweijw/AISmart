using Microsoft.Extensions.DependencyInjection;
using Orleans.Storage;

namespace AISmart.GAgent.Core;

public sealed class ContextStorageState
{
    public Dictionary<string, object?> Context { get; set; } = new();
}

public interface IContextStorageGrain : IGrainWithGuidKey
{
    Task AddContextAsync(string key, object? value);

    Task<Dictionary<string, object?>> GetContext();
}

public class ContextStorageGrain : Grain<ContextStorageState>, IContextStorageGrain
{
    private readonly IPersistentState<ContextStorageState> _context;

    private readonly IGrainStorage _grainStorage;

    public ContextStorageGrain([PersistentState("State")] IPersistentState<ContextStorageState> context)
    {
        _context = context;
    }

    public async Task AddContextAsync(string key, object? value)
    {
        if (_context.State.Context.IsNullOrEmpty())
        {
            _context.State.Context = [];
        }

        _context.State.Context[key] = value;
    }

    public Task<Dictionary<string, object?>> GetContext()
    {
        return Task.FromResult(_context.State.Context);
    }
}