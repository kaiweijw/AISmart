using System.Collections.ObjectModel;

namespace AISmart.GAgent.Autogen.DescriptionManager;

public interface IAgentDescriptionManager:IGrainWithStringKey
{
    Task AddAgentEventsAsync(Type agentType, List<Type> eventTypes);
    Task<string> GetAutoGenEventDescriptionAsync();
    Task<ReadOnlyDictionary<string, AgentDescriptionInfo>> GetAgentDescription();
}