using AISmart.Agents;
using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Agents.MarketLeader;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Application.Grains.Agents.X;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.MarketLeader;

[ImplicitStreamSubscription(CommonConstants.StreamNamespace)]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class MarketLeaderGAgent : GAgentBase<MarketLeaderAgentState, MarketLeaderGEvent>
{
    public MarketLeaderGAgent(ILogger<MarketLeaderGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    private async Task ExecuteAsync(SocialEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: Market Leader analyses content:{eventData.Content}");
        
        await PublishAsync(new ImplementationEvent
        {
            Content = eventData.Content
        });
    }

    public override async Task HandleEventAsync(EventWrapperBase item)
    {
        if (item is EventWrapper<SocialEvent> wrapper)
        {
            await ExecuteAsync(wrapper.Event);
        }
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.MarketLeaderAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
    }
}