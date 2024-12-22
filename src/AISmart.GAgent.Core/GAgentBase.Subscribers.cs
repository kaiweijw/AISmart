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
        if (_subscriptions.State.IsNullOrEmpty())
        {
            await GrainStorage.ReadStateAsync(AISmartGAgentConstants.SubscriptionsStateName, this.GetGrainId(),
                _subscriptions);
        }
    }

    protected async Task<bool> AddSubscriptionsAsync(Guid guid, IAsyncStream<EventWrapperBase> stream)
    {
        await LoadSubscriptionsAsync();
        _subscriptions.State ??= [];
        var streamIdentity = GetStreamIdentity(guid, stream);
        var result = _subscriptions.State.TryAdd(guid, streamIdentity);
        await GrainStorage.WriteStateAsync(AISmartGAgentConstants.SubscriptionsStateName, this.GetGrainId(),
            _subscriptions);
        return result;
    }
    
    protected async Task<bool> RemoveSubscriptionsAsync(Guid guid)
    {
        await LoadSubscriptionsAsync();
        if (_subscriptions.State.IsNullOrEmpty())
        {
            return false;
        }

        if (!_subscriptions.State.Remove(guid))
        {
            return false;
        }

        await GrainStorage.WriteStateAsync(AISmartGAgentConstants.SubscriptionsStateName, this.GetGrainId(),
            _subscriptions);
        return true;
    }

    protected async Task LoadPublishersAsync()
    {
        if (_publishers.State.IsNullOrEmpty())
        {
            await GrainStorage.ReadStateAsync(AISmartGAgentConstants.PublishersStateName, this.GetGrainId(),
                _publishers);
        }
    }

    protected async Task<bool> AddPublishersAsync(Guid guid, IAsyncStream<EventWrapperBase> stream)
    {
        await LoadPublishersAsync();
        _publishers.State ??= [];
        var streamIdentity = GetStreamIdentity(guid, stream);
        var result = _publishers.State.TryAdd(guid, streamIdentity);
        await GrainStorage.WriteStateAsync(AISmartGAgentConstants.PublishersStateName, this.GetGrainId(),
            _publishers);
        return result;
    }

    protected async Task<bool> RemovePublishersAsync(Guid guid)
    {
        await LoadPublishersAsync();
        if (_publishers.State.IsNullOrEmpty())
        {
            return false;
        }

        if (!_publishers.State.Remove(guid))
        {
            return false;
        }

        await GrainStorage.WriteStateAsync(AISmartGAgentConstants.PublishersStateName, this.GetGrainId(),
            _publishers);
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