using AISmart.Domain.Grains.Event;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public interface IMarketOperatorAgent : IAgent,ILocalEventHandler<MarketLeaderCreatedEvent>
{
    /// <summary>
    /// Executes a market-leading strategy asynchronously.
    /// </summary>
    /// <param name="eventData">The event to be published.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task AnalyseContentAsync(MarketLeaderCreatedEvent eventData);

    public Task CompleteAnalyseContentAsync();
}

public class MarketOperatorAgent : Agent, IMarketOperatorAgent, IAgent, ILocalEventHandler<MarketLeaderCreatedEvent>
{
    public MarketOperatorAgent(IObjectMapper objectMapper,ILocalEventBus localEventBus) : base(objectMapper,localEventBus)
    {
    }

    public async Task AnalyseContentAsync(MarketLeaderCreatedEvent eventData)
    {
        // Additional logic can be added here before executing the strategy
    }

    public async Task CompleteAnalyseContentAsync()
    {
        await PublishAsync(new MarketOperatoerCompleteEvent());
    }

    public async Task HandleEventAsync(MarketLeaderCreatedEvent eventData)
    {
        Console.WriteLine($"MarketOperatorAgent Event Received: {eventData.Content}");
        await AnalyseContentAsync(eventData);
    }
    
}