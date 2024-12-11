using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Application.Grains.Agents.Developer;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Developer;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class DeveloperAgent : GAgent<DeveloperAgentState, ImplementationEvent>,IDeveloperAgent
{
    public DeveloperAgent(ILogger<DeveloperAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(ImplementationEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: DeveloperAgent analyses content:{eventData.Content}");
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(ImplementationEvent eventData)
    {
        return Task.CompletedTask;
    }
}