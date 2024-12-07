using AISmart.Domain.Grains.Event;
using Orleans.EventSourcing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public interface IMarketPriceAgent : IGrainWithGuidKey,ILocalEventHandler<MarketLeaderCreatedEvent>,ITransientDependency
{
    /// <summary>
    /// Executes a market-leading strategy asynchronously.
    /// </summary>
    /// <param name="eventData">The event to be published.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AnalysePriceAsync(MarketLeaderCreatedEvent eventData);
}

public class MarketPriceAgent : JournaledGrain<AgentTaskState, MarketLeaderCreatedEvent>, IMarketPriceAgent
{
    private readonly ILocalEventBus _localEventBus;

    public MarketPriceAgent(ILocalEventBus localEventBus)
    {
        _localEventBus = localEventBus;
    }

    public async Task AnalysePriceAsync(MarketLeaderCreatedEvent eventData)
    {
        // Additional logic can be added here before executing the strategy
    }
    
    public async Task HandleEventAsync(MarketLeaderCreatedEvent eventData)
    {
        Console.WriteLine($"MarketPriceAgent Event Received: {eventData.Content}");
        await AnalysePriceAsync(eventData);
    }
    
}