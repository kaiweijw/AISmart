using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Dapr;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;
using Orleans.Storage;

namespace AISmart.Application.Grains.Agents.Group;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class GroupGAgent : GAgentBase<GroupAgentState, GroupGEvent>
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
        
        // Register to itself.
        var agentGuid = this.GetPrimaryKey();
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, agentGuid);
        var stream = StreamProvider.GetStream<EventWrapperBase>(streamId);
        foreach (var observer in Observers)
        {
            await stream.SubscribeAsync(observer);
        }

        TryAddPublisher(agentGuid, stream);

        State.RegisteredAgents = 0;
    }
}