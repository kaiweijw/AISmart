using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Options;
using Orleans;
using Volo.Abp.DependencyInjection;
using Xunit;

namespace AISmart.AgentsNetwork;


public interface IAgentNetworkManager:ISingletonDependency
{
    Task InitializeAgentsAsync(AgentNetworkConfigOptions config);

    IDictionary<string, IGAgent?> GetAgentInstances();

    IEnumerable<IGAgent?> GetGroupAgents();
}

public class AgentNetworkManager:IAgentNetworkManager
{
    private readonly IGrainFactory _grainFactory;
    private readonly Dictionary<string, IGAgent?> _agentInstances = new Dictionary<string, IGAgent?>();
    private readonly List<IGAgent?> _groupAgents = new List<IGAgent?>(); // List to store group  agents


    public AgentNetworkManager(IGrainFactory grainFactory)
    {
        _grainFactory = grainFactory;
    }
    
    public IDictionary<string, IGAgent?> GetAgentInstances()
    {
        return new ReadOnlyDictionary<string, IGAgent?>(_agentInstances);
    } 
    
    public IEnumerable<IGAgent?> GetGroupAgents()
    {
        return new ReadOnlyCollection<IGAgent?>(_groupAgents);
    }

    public async Task InitializeAgentsAsync(AgentNetworkConfigOptions config)
    {
        InitAgents(config);

        await InitGroupAsync(config);
    }

    private async Task InitGroupAsync(AgentNetworkConfigOptions config)
    {
        foreach (var group in config.Groups)
        {
            // Register Agents in their respective groups
            var groupGAgentName = group.AgentsList.FirstOrDefault(); // Assume the first agent is the group leader
            Debug.Assert(groupGAgentName != null, "groupGAgentName should not be null.");
            Debug.Assert(_agentInstances.ContainsKey(groupGAgentName), "Group leader agent not found.");

            var groupGAgent = _agentInstances[groupGAgentName];
            foreach (var relation in group.RelationList)
            {
                if (_agentInstances.TryGetValue(relation.To, out var agent))
                {
                    Debug.Assert(agent != null , "agent should not be null.");
                    await groupGAgent?.Register(agent)!;
                }
            }

            _groupAgents.Add(groupGAgent);
        }
    }

    private void InitAgents(AgentNetworkConfigOptions config)
    {
        // Create Agents
        foreach (var contract in config.ContractsList)
        {
            var agentType = Type.GetType(contract.AgentState);
            Debug.Assert(agentType != null, "agentType should not be null.");
            var instance = _grainFactory.GetGrain(typeof(IStateGAgent<>), Guid.NewGuid(), agentType.Namespace);
            _agentInstances[contract.Name] = instance as IGAgent;
        }
    }
}