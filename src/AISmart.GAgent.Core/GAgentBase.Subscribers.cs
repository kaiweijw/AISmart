using AISmart.Agents;
using Orleans.Streams;

namespace AISmart.GAgent.Core;

public abstract partial class GAgentBase<TState, TEvent>
{
    private readonly IGrainState<List<GrainId>> _subscribers = new GrainState<List<GrainId>>();

    private readonly IGrainState<Dictionary<Guid, StreamIdentity>> _subscriptions =
        new GrainState<Dictionary<Guid, StreamIdentity>>();

    private readonly IGrainState<Dictionary<Guid, StreamIdentity>> _publishers =
        new GrainState<Dictionary<Guid, StreamIdentity>>();

    protected async Task LoadSubscribersAsync()
    {
        if (_subscribers.State.IsNullOrEmpty())
        {
            await GrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscribersStateName, this.GetGrainId(),
                _subscribers);
        }
    }

    protected async Task AddSubscriberAsync(GrainId grainId)
    {
        await LoadSubscribersAsync();
        _subscribers.State ??= [];
        _subscribers.State.Add(grainId);
        await GrainStorage.WriteStateAsync(AISmartGAgentConstants.SubscribersStateName, this.GetGrainId(),
            _subscribers);
    }

    protected async Task LoadSubscriptionsAsync()
    {
        await LoadStateAsync(_subscriptions, AISmartGAgentConstants.SubscriptionsStateName);
    }

    protected async Task<bool> AddSubscriptionsAsync(Guid guid, IAsyncStream<EventWrapperBase> stream)
    {
        return await AddStreamAsync(guid, stream, _subscriptions, AISmartGAgentConstants.SubscriptionsStateName);
    }

    protected async Task<bool> RemoveSubscriptionsAsync(Guid guid)
    {
        return await RemoveStreamAsync(guid, _subscriptions, AISmartGAgentConstants.SubscriptionsStateName);
    }

    protected async Task LoadPublishersAsync()
    {
        await LoadStateAsync(_publishers, AISmartGAgentConstants.PublishersStateName);
    }

    protected async Task<bool> AddPublishersAsync(Guid guid, IAsyncStream<EventWrapperBase> stream)
    {
        return await AddStreamAsync(guid, stream, _publishers, AISmartGAgentConstants.PublishersStateName);
    }

    protected async Task<bool> RemovePublishersAsync(Guid guid)
    {
        return await RemoveStreamAsync(guid, _publishers, AISmartGAgentConstants.PublishersStateName);
    }

    protected async Task LoadStateAsync(IGrainState<Dictionary<Guid, StreamIdentity>> state, string stateName)
    {
        if (state.State.IsNullOrEmpty())
        {
            await GrainStorage.ReadStateAsync(stateName, this.GetGrainId(), state);
        }
    }

    private async Task<bool> AddStreamAsync(Guid guid, IAsyncStream<EventWrapperBase> stream,
        IGrainState<Dictionary<Guid, StreamIdentity>> state, string stateName)
    {
        await LoadStateAsync(state, stateName);
        state.State ??= [];
        var streamIdentity = GetStreamIdentity(guid, stream);
        var success = state.State.TryAdd(guid, streamIdentity);
        await GrainStorage.WriteStateAsync(stateName, this.GetGrainId(), state);
        return success;
    }

    private async Task<bool> RemoveStreamAsync(Guid guid, IGrainState<Dictionary<Guid, StreamIdentity>> state,
        string stateName)
    {
        await LoadStateAsync(state, stateName);
        if (state.State.IsNullOrEmpty())
        {
            return false;
        }

        if (!state.State.Remove(guid))
        {
            return false;
        }

        await GrainStorage.WriteStateAsync(stateName, this.GetGrainId(), state);
        return true;
    }

    private StreamIdentity GetStreamIdentity(Guid guid, IAsyncStream<EventWrapperBase> stream)
    {
        return new StreamIdentity(guid, stream.StreamId.GetNamespace());
    }

    private IAsyncStream<EventWrapperBase> GetStream(StreamIdentity streamIdentity)
    {
        var streamId = StreamId.Create(streamIdentity.Namespace, streamIdentity.Guid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        return stream;
    }
}