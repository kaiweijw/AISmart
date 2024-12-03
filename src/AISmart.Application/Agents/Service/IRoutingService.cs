using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public interface IRoutingService
{
    Task<bool> InvokeAgent(string agentId, List<RoleDialogModel> dialogs);
    
}