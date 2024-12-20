using System.Collections.Concurrent;
using AISmart.Agents;
using AISmart.Dapr;
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
    where TState : class, new()
    where TEvent : GEventBase
{
    protected IStreamProvider StreamProvider => this.GetStreamProvider(CommonConstants.StreamProvider);

    protected readonly ILogger Logger;
    protected readonly IGrainStorage GrainStorage;

    // need to use persistent storage to store this
    private readonly Dictionary<Guid, IAsyncStream<EventWrapperBase>> _subscriptions = new();
    private readonly Dictionary<Guid, IAsyncStream<EventWrapperBase>> _publishers = new();
    protected readonly List<EventWrapperBaseAsyncObserver> Observers = new();

    protected GAgentBase(ILogger logger, IGrainStorage grainStorage = null)
    {
        Logger = logger;
        GrainStorage = grainStorage;
    }

    public Task ActivateAsync()
    {
        //do nothing
        return Task.CompletedTask;
    }

    public async Task<bool> SubscribeTo(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        if (!_subscriptions.TryAdd(agentGuid, stream))
        {
            return false;
        }

        await SubscribeAsync(stream);
        return true;
    }

    public Task<bool> UnsubscribeFrom(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        if (!_subscriptions.ContainsKey(agentGuid))
        {
            return Task.FromResult(false);
        }

        _subscriptions.Remove(agentGuid);
        //TODO: Unsubscribe from stream
        return Task.FromResult(true);
    }

    public Task<bool> PublishTo(IGAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        return Task.FromResult(TryAddPublisher(agentGuid, stream));
    }

    protected bool TryAddPublisher(Guid agentGuid, IAsyncStream<EventWrapperBase> stream)
    {
        return _publishers.TryAdd(agentGuid, stream);
    }

    public Task<bool> UnpublishFrom(IGAgent agent)
    {
        if (!_publishers.ContainsKey(agent.GetPrimaryKey()))
        {
            return Task.FromResult(false);
        }

        _publishers.Remove(agent.GetPrimaryKey());
        return Task.FromResult(true);
    }

    public async Task Register(IGAgent agent)
    {
        var success = await agent.SubscribeTo(this);
        success = await agent.PublishTo(this) | success;

        if (!success)
        {
            return;
        }

        var guid = agent.GetPrimaryKey();

        await LoadSubscribersAsync();
        await AddSubscriberAsync(agent.GetGrainId());

        await OnRegisterAgentAsync(guid);
    }

    public async Task Unregister(IGAgent agent)
    {
        var success = await agent.UnsubscribeFrom(this);
        success = await agent.UnpublishFrom(this) | success;

        if (!success)
        {
            return;
        }

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
        if (_subscribers == null)
        {
            return new SubscribedEventListEvent
            {
                GAgentType = GetType()
            };
        }
        var gAgentList = _subscribers.State.Select(grainId => GrainFactory.GetGrain<IGAgent>(grainId)).ToList();

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
        if (_publishers.Count == 0)
        {
            return;
        }

        foreach (var publisher in _publishers.Select(kp => kp.Value))
        {
            await publisher.OnNextAsync(eventWrapper);
        }
    }

    private async Task AckAsync(EventWrapper<TEvent> eventWrapper)
    {
        // var pubAgent = _clusterClient.GetGrain<IAgent<TState>>(eventWrapper.GrainId);
        //
        // try
        // {
        //     await ExecuteAsync(eventWrapper.Event);
        // }
        // catch (Exception e)
        // {
        //     Logger.LogError(e, "Error executing event");
        // }
        //
        // await ((pubAgent as GAgent<TState,TEvent>)!).DoAckAsync(eventWrapper);
    }

    private async Task DoAckAsync(EventWrapper<TEvent> eventWrapper)
    {
        // eventWrapper.count ++;
        //
        // RaiseEvent(eventWrapper.Event);
        // await ConfirmEvents();
        //
        // var stream = this.GetStreamProvider(CommonConstants.StreamProvider)
        //     .GetStream<EventWrapperBase>(StreamId);
        // var subscriptionHandles =  stream.GetAllSubscriptionHandles();
        // // The count of current subscriptions (consumers).
        // var subscriberCount = subscriptionHandles.Result.Count;
        //
        // if (eventWrapper.count == subscriberCount)
        // {
        //     await CompleteAsync(eventWrapper.Event);
        // }
    }




    // Agent3 -> Agent2 -> Agent3 dependencies through messages
    // strong typed messages
    // 3 messages sub -> 2 messages pub -> 3 messages pub

    // twitter and telegram as a agent
    // twitter and telegram agent publish message
    // market leader agent subscribe to twitter and telegram agent
    // market leader agent publish message to agentA and agentB
    // agentA (investment) and agentB (developer) subscribe to market leader agent
    // market leader agent waits for agentA and agentB to complete
    // market leader agent execute something
    // market leader agent publish message to twitter and telegram agent

    // variation 2
    // agentB sends to agentC to do transaction on chain

    // how does dependency work between agents? just pub/sub messages enough?

    private async Task SubscribeAsync(IAsyncStream<EventWrapperBase> stream)
    {
        foreach (var observer in Observers)
        {
            await stream.SubscribeAsync(observer);
        }
    }

    private async Task HandleEventAsync(EventWrapperBase item)
    {
        Logger.LogInformation("Received message: {@Message}", item);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await UpdateObserverList();
    }
}