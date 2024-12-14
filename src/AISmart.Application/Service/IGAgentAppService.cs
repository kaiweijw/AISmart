using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Application.Grains;
using Orleans;
using Volo.Abp.Application.Services;
using Volo.Abp.Auditing;

namespace AISmart.Service;

[DisableAuditing]
public class GAgentAppService(IClusterClient clusterClient) : ApplicationService, IGAgentAppService
{
    public async Task<Guid> CreateAgentAsync(string agentType)
    {
        var agentId = Guid.NewGuid();
        var agent = clusterClient.GetGrain<IAgent>(agentId, agentType);
        return agentId;
    }

    public async Task RegisterAgentToGroupAsync(Guid agentId, Guid groupId)
    {
        var groupAgent = clusterClient.GetGrain<IStateAgent<GroupAgentState>>(groupId);
        var agent = clusterClient.GetGrain<IAgent>(agentId);
        await groupAgent.Register(agent);
    }

    public Task<List<string?>> GetAllAvailableGAgentListAsync()
    {
        var agentType = typeof(GAgentBase<,>);
        var agentTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == agentType)
            .Select(t => t.FullName)
            .ToList();

        return Task.FromResult(agentTypes);
    }
}
