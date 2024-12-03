using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public class RoutingService : IRoutingService
{
    
    
    
    public async Task<AgentTask> InvokeAgent(string agentId, Event poppedEvent)
    {
        // var agent = await agentService.LoadAgent(agentId);
        //
        // var provider = agent.LlmConfig.Provider;
        // var model = agent.LlmConfig.Model;
        //
        // // call open api; 
        // var response = await openai.chatCompeltion(agent, dialogs);
        // dialogs.Add(response);

        return new AgentTask()
        {
            Id = Guid.NewGuid().ToString(),
            Content = poppedEvent.Data,
            success = false
        };
    }

    public async Task<AgentTask> EvaluateAgent(string agentId, AgentTask AgentTask)
    {
        AgentTask.success = true;
        return AgentTask;
    }

    public List<AgentTask> SplitAgentTask(string agentId, Event poppedEvent)
    {
        return new List<AgentTask>()
        {
            new AgentTask()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "to call wallet agent",
                Content = poppedEvent.Data
            },
            new AgentTask()
            {
                Id = Guid.NewGuid().ToString(),
                Description = "to notify telegram",
                Content = poppedEvent.Data
            }
        };
    }

    public Agent RoutingAgent(string agentId, Event poppedEvent)
    {
        return new Agent()
        {
            Id = Guid.NewGuid().ToString()
        };
    }
}