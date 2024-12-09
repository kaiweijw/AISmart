using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Orleans.EventSourcing;
using Orleans.Runtime;
using Orleans.Streams;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;


public interface IMarketLeaderStreamAgent :  IGrainWithGuidKey
{
    /// <summary>
    /// Executes a market-leading strategy asynchronously.
    /// </summary>
    /// <param name="eventData">The event to be published.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task ExecuteStrategyAsync(TelegramEvent eventData);

    public Task CompelteStrategyAsync(MarketOperatoerCompleteEvent eventData);
}


public class MarketLeaderStreamAgent : Grain<BasicEvent>, IMarketLeaderStreamAgent
{
    

    public MarketLeaderStreamAgent()
    {
    }

    private IAsyncStream<BasicEvent>? _stream;

    
    public async Task ExecuteStrategyAsync(TelegramEvent eventData)
    {
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

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, CommonConstants.StreamGuid);
        _stream = this
            .GetStreamProvider(CommonConstants.StreamProvider)
            .GetStream<BasicEvent>(streamId);
        
        await _stream.SubscribeAsync(OnNextAsync);

    }
    
    private Task OnNextAsync(BasicEvent message, StreamSequenceToken token = null)
    {
        Console.WriteLine($"OrleansMarketLeaderAgent Received message: {message.Content}");
        return Task.CompletedTask;
    }
}