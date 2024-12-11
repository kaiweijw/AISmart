using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Application.Grains.Agents.Developer;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.Developer;

public class DeveloperAgent : GAgent<DeveloperAgentState, DeveloperEvent>
{
    public DeveloperAgent(ILogger<DeveloperAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task ExecuteAsync(DeveloperEvent eventData)
    {
        Logger.LogInformation($"{this.GetType().ToString()} ExecuteAsync: Market Leader analyses content:{eventData.Content}");
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(DeveloperEvent eventData)
    {
        return Task.CompletedTask;
    }
}