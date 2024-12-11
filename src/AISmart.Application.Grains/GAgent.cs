using AISmart.Agents;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;
using Volo.Abp.Domain.Entities.Events;

namespace AISmart.Application.Grains;

public abstract class GAgent<TState, TEvent> : JournaledGrain<TState, TEvent>, IAgent<TEvent>
    where TState : class, new()
    where TEvent : class, new()
{
    protected IStreamProvider? StreamProvider { get; private set; } = null;
    protected ILogger Logger { get; }
    private StreamId StreamId { get; set; }
    
    private readonly IClusterClient _clusterClient;
    
    private IDisposable? _timer;


    
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
        StreamId = StreamId.Create(CommonConstants.StreamNamespace, Guid.NewGuid());
        StreamProvider = this.GetStreamProvider(CommonConstants.StreamProvider);
    }



    public Task SubscribeAsync(IAgent<TEvent> agent)
    {
        var streamId = (agent as GAgent<TState,TEvent>).GetStreamId();
        var stream = this.GetStreamProvider(CommonConstants.StreamProvider)
            .GetStream<TEvent>(streamId);
        stream.SubscribeAsync(OnNextAsync as Func<TEvent, StreamSequenceToken, Task>);
        return Task.CompletedTask;
    }
    
    protected async Task PublishAsync<TEvent>(TEvent @event)
    {
        var stream = StreamProvider?.GetStream<EventWrapper<TEvent>>(StreamId);

        if (stream == null)
        {
            Logger.LogError("StreamProvider is null");
            return;
        }
        
        var subscriptionHandles =  stream.GetAllSubscriptionHandles();
        // The count of current subscriptions (consumers).
        var subscriberCount = subscriptionHandles.Result.Count;
        
        EventWrapper<TEvent> eventWrapper = new EventWrapper<TEvent>(@event, StreamId,this.GetGrainId(),subscriberCount);
        
        await stream.OnNextAsync(eventWrapper);
        
        

    }

    public async Task AckAsync(EventWrapper<TEvent> eventWrapper)
    {

        StreamId = eventWrapper.StreamId;
        var stream = this.GetStreamProvider(CommonConstants.StreamProvider)
        .GetStream<EventWrapper<TEvent> >(StreamId);

        IAgent<TEvent> pubAgent = _clusterClient.GetGrain<IAgent<TEvent>>(eventWrapper.GrainId);

        ExecuteAsync(eventWrapper.Event);

        await ((pubAgent as GAgent<TState,TEvent>)!).DoACKAysnc(eventWrapper);
    }

    public async Task DoACKAysnc(EventWrapper<TEvent> eventWrapper)
    {
        using var cts = new CancellationTokenSource();
        
        cts.CancelAfter(CommonConstants.TimeOutMilliseconds);
        
        
        eventWrapper.Count ++;
        
        RaiseEvent(eventWrapper.Event);
        await ConfirmEvents();

        if (eventWrapper.Count == eventWrapper.SubscriberCount)
        {
            await CompleteAsync(eventWrapper.Event);
            eventWrapper.Success = true;
        }
        else
        {
            var period = TimeSpan.FromSeconds(60);
            RegisterTimer(
                asyncCallback: async _ => await TimerTick(),
                state: null,
                dueTime: period,
                period: period);
        }

    }

    
    private async Task TimerTick()
    {
        var historyEvents =  this.RetrieveConfirmedEvents(Version-1,Version);   
        // todo to Retrieve eventWrapper 
        if (historyEvents.Result[0].Equals("to do "))
        {
            Console.WriteLine("eventWrapper has been completed");
        }
        else
        {
            await CompleteAsync(new TEvent());
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

    
    private Task OnNextAsync(EventWrapper<TEvent> @event, StreamSequenceToken token = null)
    {
        Logger.LogInformation("Received message: {@Message}", @event);

        ExecuteAsync(@event.Event);
        DoACKAysnc(@event);
        return Task.CompletedTask;
    }
}