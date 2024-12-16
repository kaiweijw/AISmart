using System.Reflection;
using AISmart.Agents;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;

namespace AISmart.Application.Grains;

public abstract class GAgentBase<TState, TEvent> : JournaledGrain<TState, TEvent>, IStateAgent<TState>
    where TState : class, new()
    where TEvent : GEvent
{
    private IStreamProvider StreamProvider => this.GetStreamProvider(CommonConstants.StreamProvider);

    protected readonly ILogger Logger;

    // need to use persistent storage to store this
    private readonly Dictionary<Guid, IAsyncStream<EventWrapperBase>> _subscriptions = new();
    private readonly Dictionary<Guid, IAsyncStream<EventWrapperBase>> _publishers = new();
    private readonly List<EventWrapperBaseAsyncObserver> _observers = new();

    protected GAgentBase(ILogger logger)
    {
        Logger = logger;
    }

    public Task ActivateAsync()
    {
        //do nothing
        return Task.CompletedTask;
    }

    public async Task<bool> SubscribeTo(IAgent agent)
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

    public Task<bool> UnsubscribeFrom(IAgent agent)
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

    public Task<bool> PublishTo(IAgent agent)
    {
        var agentGuid = agent.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        return Task.FromResult(_publishers.TryAdd(agentGuid, stream));
    }

    public Task<bool> UnpublishFrom(IAgent agent)
    {
        if (!_publishers.ContainsKey(agent.GetPrimaryKey()))
        {
            return Task.FromResult(false);
        }

        _publishers.Remove(agent.GetPrimaryKey());
        return Task.FromResult(true);
    }

    public async Task Register(IAgent agent)
    {
        var success = await agent.SubscribeTo(this);
        success = await agent.PublishTo(this) | success;
    
        if (!success)
        {
            return;
        }
    
        await OnRegisterAgentAsync(agent.GetPrimaryKey());
    }
    
    public async Task Unregister(IAgent agent)
    {
        var success = await agent.UnsubscribeFrom(this);
        success = await agent.UnpublishFrom(this) | success;
    
        if (!success)
        {
            return;
        }
    
        await OnUnregisterAgentAsync(agent.GetPrimaryKey());
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

    protected async Task PublishAsync<T>(T @event) where T : EventBase
    {
        if (_publishers.Count == 0)
        {
            return;
        }

        var eventWrapper = new EventWrapper<T>(@event, this.GetGrainId());

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
        foreach (var observer in _observers)
        {
            await stream.SubscribeAsync(observer);
        }
    }

    private async Task HandleEventAsync(EventWrapperBase item, StreamSequenceToken? token)
    {
        Logger.LogInformation("Received message: {@Message}", item);
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var methods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(m => m.GetCustomAttribute<EventHandlerAttribute>() != null || m.Name == nameof(HandleEventAsync))
            .Where(m => m.ReturnType == typeof(Task) && m.GetParameters().Length == 1 &&
                        typeof(EventBase).IsAssignableFrom(m.GetParameters()[0].ParameterType));

        foreach (var method in methods)
        {
            var observer = new EventWrapperBaseAsyncObserver(async item =>
            {
                var eventType = item.GetType().GetProperty(nameof(EventWrapper<object>.Event))?.GetValue(item);
                if (method.GetParameters()[0].ParameterType == eventType!.GetType())
                {
                    await (Task)method.Invoke(this, [eventType])!;
                }
            });

            _observers.Add(observer);
        }
    }
}