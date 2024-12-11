using AISmart.Application.Grains.Agents.MarketLeader.Events;
using AISmart.Dapr;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.MarketLeader;

public class MarketLeaderAgent : GAgent<MarketLeaderAgentState, SocialEvent>
{
    public MarketLeaderAgent(ILogger logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(SocialEvent eventData)
    {
        Console.WriteLine("ExecuteAsync: Social Event {Content}", eventData.Content);
        return Task.CompletedTask;
    }
}