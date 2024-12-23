using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Dapr;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.Group;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class GroupGAgent : EventHandler<GroupAgentState, GroupGEvent>
{
    public GroupGAgent(ILogger<GroupGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a social event is published.");
    }

    protected override Task OnRegisterAgentAsync(Guid agentGuid)
    {
        ++State.RegisteredAgents;
        return Task.CompletedTask;
    }

    protected override Task OnUnregisterAgentAsync(Guid agentGuid)
    {
        --State.RegisteredAgents;
        return Task.CompletedTask;
    }
    
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        State.RegisteredAgents = 0;
    }
}