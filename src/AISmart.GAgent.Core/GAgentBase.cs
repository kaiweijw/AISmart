using System.Collections.Concurrent;
using AISmart.Agents;
using AISmart.Dapr;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Storage;
using Orleans.Streams;

namespace AISmart.GAgent.Core;

[GAgent]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public abstract partial class GAgentBase<TState, TEvent> : JournaledGrain<TState, TEvent>, IStateGAgent<TState>
    where TState : StateBase, new()
    where TEvent : GEventBase
{
    protected IStreamProvider StreamProvider => this.GetStreamProvider(CommonConstants.StreamProvider);

    protected readonly ILogger Logger;
    protected readonly IGrainStorage GrainStorage;

    /// <summary>
    /// Observer -> HandleId
    /// </summary>
    protected readonly Dictionary<EventWrapperBaseAsyncObserver, Guid> Observers = new();
    private IEventDispatcher EventDispatcher { get; set; }

    protected GAgentBase(ILogger logger)
    {
        Logger = logger;
        GrainStorage = ServiceProvider.GetRequiredService<IGrainStorage>();
        EventDispatcher = ServiceProvider.GetRequiredService<IEventDispatcher>();
    }

    public Task ActivateAsync()
    {
        //do nothing
        return Task.CompletedTask;
    }

    public async Task<bool> SubscribeToAsync(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        if (!await AddSubscriptionsAsync(agentGuid, stream))
        {
            return false;
        }

        await SubscribeAsync(stream);
        return true;
    }

    public async Task<bool> UnsubscribeFromAsync(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        if (await RemoveSubscriptionsAsync(agentGuid))
        {
            var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
            var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
            var handlers = await stream.GetAllSubscriptionHandles();
            foreach (var handle in handlers.Where(h => Observers.ContainsValue(h.HandleId)))
            {
                await handle.UnsubscribeAsync();
            }

            return true;
        }

        return false;
    }

    public async Task<bool> PublishToAsync(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        return await AddPublishersAsync(agentGuid, stream);
    }

    public async Task<bool> UnpublishFromAsync(IGAgent agent)
    {
        return await RemovePublishersAsync(agent.GetPrimaryKey());
    }

    public async Task RegisterAsync(IGAgent agent)
    {
        var success = await agent.SubscribeToAsync(this);
        success = await agent.PublishToAsync(this) | success;

        if (!success)
        {
            return;
        }

        var guid = agent.GetPrimaryKey();

        await AddSubscriberAsync(agent.GetGrainId());

        await OnRegisterAgentAsync(guid);
    }

    public async Task UnregisterAsync(IGAgent agent)
    {
        var success = await agent.UnsubscribeFromAsync(this);
        success = await agent.UnpublishFromAsync(this) | success;

        if (!success)
        {
            return;
        }

        await RemoveSubscriberAsync(agent.GetGrainId());

        await OnUnregisterAgentAsync(agent.GetPrimaryKey());
    }

    public async Task<List<Type>?> GetAllSubscribedEventsAsync()
    {
        var eventHandlerMethods = GetEventHandlerMethods();
        var handlingTypes = eventHandlerMethods
            .Select(m => m.GetParameters().First().ParameterType)
            .Where(t => t != typeof(RequestAllSubscriptionsEvent))
            .ToList();
        return handlingTypes;
    }

    [EventHandler]
    public async Task<SubscribedEventListEvent> HandleRequestAllSubscriptionsEventAsync(RequestAllSubscriptionsEvent request)
    {
        await LoadSubscribersAsync();

        var gAgentList = _subscribers.State.Select(grainId => GrainFactory.GetGrain<IGAgent>(grainId)).ToList();

        if (gAgentList.IsNullOrEmpty())
        {
            return new SubscribedEventListEvent
            {
                GAgentType = GetType()
            };
        }

        if (gAgentList.Any(grain => grain == null))
        {
            // Only happened on test environment.
            throw new InvalidOperationException("One or more grains in gAgentList are null.");
        }

        var dict = new ConcurrentDictionary<Type, List<Type>>();
        foreach (var gAgent in gAgentList.AsParallel())
        {
            var eventList = await gAgent.GetAllSubscribedEventsAsync();
            dict[gAgent.GetType()] = eventList ?? [];
        }

        return new SubscribedEventListEvent
        {
            Value = dict.ToDictionary(),
            GAgentType = GetType()
        };
    }

    protected virtual async Task OnRegisterAgentAsync(Guid agentGuid)
    {
    }

    protected virtual async Task OnUnregisterAgentAsync(Guid agentGuid)
    {
    }

    public abstract Task<string> GetDescriptionAsync();

    public Task<TState> GetStateAsync()
    {
        return Task.FromResult(State);
    }

    protected async Task<Guid> PublishAsync<T>(T @event) where T : EventBase
    {
        var eventId = Guid.NewGuid();
        var eventWrapper = new EventWrapper<T>(@event, eventId, this.GetPrimaryKey());

        await PublishAsync(eventWrapper);

        return eventId;
    }

    private async Task PublishAsync<T>(EventWrapper<T> eventWrapper) where T : EventBase
    {
        await LoadPublishersAsync();
        if (_publishers.State.Count == 0)
        {
            return;
        }

        foreach (var publisher in _publishers.State.Select(kp => kp.Value))
        {
            var stream = GetStream(publisher);
            await stream.OnNextAsync(eventWrapper);
        }
    }

    private async Task SubscribeAsync(IAsyncStream<EventWrapperBase> stream)
    {
        foreach (var observer in Observers.Keys)
        {
            var handle = await stream.SubscribeAsync(observer);
            var handleId = handle.HandleId;
            Observers[observer] = handleId;
        }
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateObserverList();
    }
    
    protected virtual async Task HandleStateChangedAsync()
    {
    }

    protected sealed override async void OnStateChanged()
    {
        HandleStateChangedAsync();

        //TODO:  need optimize use kafka,ensure Es written successfully
        await EventDispatcher.PublishAsync(State, this.GetGrainId().ToString());
    }
}