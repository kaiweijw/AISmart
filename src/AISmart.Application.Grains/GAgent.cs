using AISmart.Agents;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;

namespace AISmart.Application.Grains;

public abstract class GAgent<TState, TEvent> : JournaledGrain<TState, TEvent>, IAgent<TEvent>
    where TState : class, new()
    where TEvent : class
{
    protected IStreamProvider? StreamProvider { get; private set; } = null;
    protected ILogger Logger { get; }
    private StreamId StreamId { get; set; }
    
    protected GAgent(ILogger logger)
    {
        Logger = logger;
    }
    
    public Task ActivateAsync()
    {
        //do nothing
        return Task.CompletedTask;
    }
    
    public abstract Task<string> GetDescriptionAsync();
    public StreamId GetStreamId()
    {
        return StreamId;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        StreamId = StreamId.Create(CommonConstants.StreamNamespace, Guid.NewGuid());
        StreamProvider = this.GetStreamProvider(CommonConstants.StreamProvider);
    }
    
    
    
    public Task SubscribeAsync(IAgent<TEvent> agent)
    {
        var streamId = agent.GetStreamId();
        var stream = this.GetStreamProvider(CommonConstants.StreamProvider)
            .GetStream<TEvent>(streamId);
        stream.SubscribeAsync(OnNextAsync);
        return Task.CompletedTask;
    }
    
    protected async Task PublishAsync<T>(T @event)
    {
        var stream = StreamProvider?.GetStream<T>(StreamId);

        if (stream == null)
        {
            Logger.LogError("StreamProvider is null");
            return;
        }

        await stream.OnNextAsync(@event);
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
    
    private Task OnNextAsync(TEvent eventData, StreamSequenceToken token = null)
    {
        Logger.LogInformation("Received message: {@Message}", eventData);
        
        ExecuteAsync(eventData);
        
        return Task.CompletedTask;
    }
}