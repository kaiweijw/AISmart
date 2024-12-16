using AISmart.Agents;

namespace AISmart.GAgent.Autogen;

public interface IAutogenAgent : IGAgent
{
    void RegisterAgentEvent(Type agent, List<Type> eventTypes);
}