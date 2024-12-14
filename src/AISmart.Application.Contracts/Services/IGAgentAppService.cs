using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AISmart.Service;

public interface IGAgentAppService
{
    Task<Guid> CreateAgentAsync(string agentType);
    Task<List<string?>> GetAllAvailableGAgentListAsync();
}
