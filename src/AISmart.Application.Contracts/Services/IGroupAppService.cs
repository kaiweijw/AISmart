using System;
using System.Threading.Tasks;

namespace AISmart.Service;

public interface IGroupAppService
{
    Task<Guid> CreateGroupAsync();
    Task SubscribeAgentToGroupAsync(Guid agentId, string agentType, Guid groupId);
}
