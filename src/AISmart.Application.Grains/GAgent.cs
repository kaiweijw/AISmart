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

    protected async Task PublishAsync<T>(T @event) where T : GEvent
    {
        var stream = StreamProvider?.GetStream<EventWrapperBase>(StreamId);

        if (stream == null)
        {
            Logger.LogError("StreamProvider is null");
            return;
        }
        var eventWrapper = new EventWrapper<T>(@event, this.GetGrainId());
        
        await stream.OnNextAsync(eventWrapper);

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
        if(@event is EventWrapper<TEvent> eventWrapper)
        {
            Logger.LogInformation("Received EventWrapper message: {@Message}", eventWrapper);

            await ExecuteAsync(eventWrapper.Event);
            await DoAckAsync(eventWrapper);
        }
    }
}