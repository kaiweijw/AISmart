using AISmart.Agents;

namespace AISmart.GAgent.Core;

public interface IEventDispatcher
{
    Task PublishAsync(StateBase state, string id);
}