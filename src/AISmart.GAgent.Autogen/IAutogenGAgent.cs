using AISmart.Agents;

namespace AISmart.GAgent.Autogen;

public interface IAutogenGAgent : IGAgent
{
    Task RegisterAgentEvent(Type agent, List<Type> eventTypes);
}