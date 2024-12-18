using System.ComponentModel;
using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Agents.MarketLeader;
using AISmart.Agents.MarketLeader.Events;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.MarketLeader;
[Description("Handle Marketing Tasks")]
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

    public async Task<ImplementationEvent> HandleEventAsync(SocialEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: Market Leader analyses content:{eventData.Content}");
        
        return new ImplementationEvent
        {
            Content = "Market execution has been completed."
        };
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.MarketLeaderAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
    }
}