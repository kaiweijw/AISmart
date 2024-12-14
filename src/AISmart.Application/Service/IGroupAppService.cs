using System;
using System.Threading.Tasks;
using AISmart.Agents.Group;
using AISmart.Service;
using Orleans;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;

namespace AISmart.Agents.Service;

[DisableAuditing]
public class GroupAppService : ApplicationService, IGroupAppService
{
    private readonly IClusterClient _clusterClient;

    public GroupAppService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task<Guid> CreateGroupAsync()
    {
        var groupId = Guid.NewGuid();
        var groupGAgent = _clusterClient.GetGrain<IStateAgent<GroupAgentState>>(groupId);
        return groupId;
    }

    public async Task SubscribeAgentToGroupAsync(Guid agentId, string agentType, Guid groupId)
    {
        var groupGAgent = _clusterClient.GetGrain<IStateAgent<GroupAgentState>>(groupId);
        var agent = _clusterClient.GetGrain<IAgent>(agentId, agentType);
        await groupGAgent.Register(agent);
    }
}
