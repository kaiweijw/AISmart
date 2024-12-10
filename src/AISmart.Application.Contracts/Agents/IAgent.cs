using System.Threading.Tasks;

namespace AISmart.Agents;

public interface IAgent<TEvent>
    where TEvent : class
{
    Task ExecuteAsync(TEvent eventData);
    Task PublishAsync<T>(T @event);
}