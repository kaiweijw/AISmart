using System.Threading.Tasks;

namespace AISmart.Agents.Service;

public interface IAgentService
{
    Task<Agent> CreateAgent(Agent agent);
    
    /// <summary>
    /// Load agent configurations and trigger hooks
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<Agent> LoadAgent(string id);
}