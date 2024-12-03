using System;
using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public class AgentService:IAgentService
{
    public Task<Agent> CreateAgent()
    {
        throw new System.NotImplementedException();
    }

    public async Task<Agent> CreateAgent(AgentGoal agentGoal)
    {
        return new Agent()
        {
            Id = Guid.NewGuid().ToString()
        };
    }

    public Task<Agent> CreateAgent(Agent agent)
    {
        throw new System.NotImplementedException();
    }

    public Task<Agent> LoadAgent(string id)
    {
        throw new System.NotImplementedException();
    }
}