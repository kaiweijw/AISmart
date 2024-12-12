using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Application.Grains.Agents.MarketLeader;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Investment;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class InvestmentAgent : GAgent<InvestmentAgentState, ImplementationEvent>, IInvestmentAgent<InvestmentAgentState>
{
    public InvestmentAgent(ILogger<InvestmentAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    public Task<InvestmentAgentState> GetStateAsync()
    {
        return Task.FromResult(State);
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