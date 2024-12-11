using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.MarketLeader;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.Investment;

public class InvestmentAgent : GAgent<InvestmentAgentState, ImplementationEvent>,IInvestmentAgent
{
    public InvestmentAgent(ILogger<InvestmentAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(ImplementationEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: InvestmentAgent analyses content:{eventData.Content}");
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(ImplementationEvent eventData)
    {
        return Task.CompletedTask;
    }
}