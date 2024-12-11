using AISmart.Agents.MarketLeader.Events;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.MarketLeader;

public class MarketLeaderAgent : GAgent<MarketLeaderAgentState, SocialEvent>
{
    public MarketLeaderAgent(ILogger<MarketLeaderAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(SocialEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: Market Leader analyses content:{eventData.Content}");
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(SocialEvent eventData)
    {
        throw new NotImplementedException();
    }
}