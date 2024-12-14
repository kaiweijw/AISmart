using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Agents.Investment;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Investment;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class InvestmentGAgent : GAgentBase<InvestmentAgentState, InvestmentGEvent>, IInvestmentStateAgent<InvestmentAgentState>
{
    public InvestmentGAgent(ILogger<InvestmentGAgent> logger) : base(logger)
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

    private Task ExecuteAsync(ImplementationEvent eventData)
    {
        if (State.Content.IsNullOrEmpty())
        {
            State.Content = [];
        }

        State.Content.Add(eventData.Content);
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: InvestmentAgent analyses content:{eventData.Content}");
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        GrainTracker.InvestmentAgents.Enqueue(this);
        await base.OnActivateAsync(cancellationToken);
        await SubscribeAsync<ImplementationEvent>(ExecuteAsync);
    }
}