using System.Reflection;
using AISmart.Agents;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;

namespace AISmart.Application.Grains;

public abstract class GAgent<TState, TEvent> : JournaledGrain<TState, TEvent>, IAgent
    where TState : class, new()
    where TEvent : GEvent
{
    protected IStreamProvider? StreamProvider { get; private set; } = null;
    
    protected readonly ILogger Logger;
    private StreamId StreamId { get; set; }
    
    private readonly IClusterClient _clusterClient;

    
    protected GAgent(ILogger logger,IClusterClient clusterClient)
    {
        Logger = logger;
        _clusterClient = clusterClient;
    }
    
    public Task ActivateAsync()
    {
        //do nothing
        return Task.CompletedTask;
    }
    
    public abstract Task<string> GetDescriptionAsync();
    private StreamId GetStreamId()
    {
        return StreamId;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        StreamId = StreamId.Create(CommonConstants.StreamNamespace, CommonConstants.StreamGuid);
        StreamProvider = this.GetStreamProvider(CommonConstants.StreamProvider);
        var stream = StreamProvider.GetStream<EventWrapperBase>(StreamId);
        
        await stream.SubscribeAsync(OnNextAsync);
    }


    private Dictionary<Type, IAsyncStream<EventWrapperBase>> dictionary { get; } = new Dictionary<Type, IAsyncStream<EventWrapperBase>>();


    protected async Task PublishAsync<T>(T @event)
    {
        if (dictionary.TryGetValue(typeof(T), out IAsyncStream<EventWrapperBase> stream) )
        {
        }
        else
        {
            Logger.LogError("StreamProvider is null");
            stream = StreamProvider?.GetStream<EventWrapperBase>(typeof(T).FullName);
            dictionary.Add(typeof(T),stream);
        }
        
        var eventWrapper = new EventWrapper<T>(@event, this.GetGrainId());
        
        await stream.OnNextAsync(eventWrapper);
        
    }

    public Task SubscribeAsync<T>(T @event)
    {

        if (dictionary.TryGetValue(typeof(T), out IAsyncStream<EventWrapperBase> stream) )
        {
            stream.SubscribeAsync(OnNextAsync);
        }

        return Task.CompletedTask;
    }
    
    
    
    
    protected async Task PublishAsync2<T>(T @event)
    {
       
        Logger.LogError("StreamProvider is null");
        var stream = StreamProvider?.GetStream<EventWrapperBase>(this.GetGrainId().ToString());

        var eventWrapper = new EventWrapper<T>(@event, this.GetGrainId());
        await stream.OnNextAsync(eventWrapper);
        
    }

    public Task SubscribeAsync2(IAgent agent)
    {
        
        var stream = StreamProvider?.GetStream<EventWrapperBase>(agent.GetGrainId().ToString());

        stream.SubscribeAsync(OnNextAsync);

        return Task.CompletedTask;
    }

    
    

    public async Task AckAsync(EventWrapper<TEvent> eventWrapper)
    {
        var pubAgent = _clusterClient.GetGrain<IAgent>(eventWrapper.GrainId);

        try
        {
            await ExecuteAsync(eventWrapper.Event);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Error executing event");
        }

        await ((pubAgent as GAgent<TState,TEvent>)!).DoAckAsync(eventWrapper);
    }

    public async Task DoAckAsync(EventWrapper<TEvent> eventWrapper)
    {
        eventWrapper.count ++;
        
        RaiseEvent(eventWrapper.Event);
        await ConfirmEvents();

        var stream = this.GetStreamProvider(CommonConstants.StreamProvider)
            .GetStream<EventWrapperBase>(StreamId);
        var subscriptionHandles =  stream.GetAllSubscriptionHandles();
        // The count of current subscriptions (consumers).
        var subscriberCount = subscriptionHandles.Result.Count;
        
        if (eventWrapper.count == subscriberCount)
        {
            await CompleteAsync(eventWrapper.Event);
        }
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
    

    protected abstract Task ExecuteAsync(TEvent eventData);


    protected abstract Task CompleteAsync(TEvent eventData);


    private async Task OnNextAsync(EventWrapperBase @event, StreamSequenceToken token = null)
    {
        Logger.LogInformation("Received message: {@Message}", @event);

        var eventWrapperType = @event.GetType();
        if (eventWrapperType.IsGenericType && eventWrapperType.GetGenericTypeDefinition() == typeof(EventWrapper<>))
        {

            var methods = this.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.Name == "ExecuteAsync");


            var eventProperty = eventWrapperType.GetProperty("Event");

            if (eventProperty != null)
            {
                var eventType = eventProperty.PropertyType;

                foreach (var method in methods)
                {
                    var parameters = method.GetParameters();

                    if (parameters.Length == 1 && parameters[0].ParameterType == eventType)
                    {
                        await (Task)method.Invoke(this, new object[] { eventProperty });
                        return;
                    }
                }
            }
        }
    }
}