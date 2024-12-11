using AISmart.Agents.MarketLeader.Events;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.MarketLeader;

public class MarketLeaderAgent : GAgent<MarketLeaderAgentState, SocialEvent>
{
    public MarketLeaderAgent(ILogger logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(SocialEvent eventData)
    {
        Console.WriteLine("{0} ExecuteAsync: Market Leader analyses content: {1}", this.GetType().ToString(), eventData.Content);
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(SocialEvent eventData)
    {
        throw new NotImplementedException();
    }
}