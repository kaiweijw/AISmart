using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public class RoutingService : IRoutingService
{
    
    
    
    public Task<bool> InvokeAgent(string agentId, List<RoleDialogModel> dialogs)
    {
        var agent = await agentService.LoadAgent(agentId);
        
        var provider = agent.LlmConfig.Provider;
        var model = agent.LlmConfig.Model;
        
        // call open api; 
        var response = await openai.chatCompeltion(agent, dialogs);
        dialogs.Add(response);

        return true;
    }
}