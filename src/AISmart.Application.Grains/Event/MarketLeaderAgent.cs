using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Orleans.EventSourcing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public interface IMarketLeaderAgent :  IGrainWithGuidKey,ILocalEventHandler<TelegramEvent>,
    ILocalEventHandler<MarketOperatoerCompleteEvent>,ITransientDependency
{
    /// <summary>
    /// Executes a market-leading strategy asynchronously.
    /// </summary>
    /// <param name="eventData">The event to be published.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExecuteStrategyAsync(TelegramEvent eventData);

    public Task CompelteStrategyAsync(MarketOperatoerCompleteEvent eventData);
}

public class MarketLeaderAgent : JournaledGrain<AgentTaskState, TelegramEvent>, IMarketLeaderAgent
{
    
    private readonly ILocalEventBus _localEventBus;

    
    public MarketLeaderAgent(IObjectMapper objectMapper, ILocalEventBus localEventBus)
    {
        _localEventBus = localEventBus;
    }

    public async Task ExecuteStrategyAsync(TelegramEvent eventData)
    {
        // Additional logic can be added here before executing the strategy
        eventData.State = EventStateEnum.Processing;
        base.RaiseEvent(eventData);
        await ConfirmEvents();
        
        // call autogen , produces new event;
        MarketLeaderCreatedEvent marketLeaderCreatedEvent = new MarketLeaderCreatedEvent
        {
            Id = Guid.NewGuid(),
            AgentTopic = CommonConstants.GptTopic,
            Downstreams = null,
            Content = "请分析《比特币突破10万美元大关》对市场的影响"
        };
        await _localEventBus.PublishAsync(marketLeaderCreatedEvent);
    }

    public Task CompelteStrategyAsync(MarketOperatoerCompleteEvent eventData)
    {
        return Task.CompletedTask;
    }

    public Task HandleEventAsync(TelegramEvent eventData)
    {
        // this.Apply(new ChatEvent { id = eventData.Id });
        Console.WriteLine($"MarketLeaderAgent Event Received: {eventData.Content}");
        ExecuteStrategyAsync(eventData);
        return Task.CompletedTask;
    }

    public Task HandleEventAsync(MarketOperatoerCompleteEvent eventData)
    {
        Console.WriteLine($"MarketLeaderAgent Event Received: {eventData.Content}");
        CompelteStrategyAsync(eventData);
        return Task.CompletedTask;
    }
}