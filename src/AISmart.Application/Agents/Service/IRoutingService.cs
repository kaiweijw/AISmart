using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public interface IRoutingService
{
    
    List<AgentTask> SplitAgentTask(string agentId, Event poppedEvent);
    Agent RoutingAgent(string agentId, Event poppedEvent);
    
    Task<AgentTask> InvokeAgent(string agentId, Event poppedEvent);
    
    Task<AgentTask> EvaluateAgent(string agentId, AgentTask AgentTask);
}