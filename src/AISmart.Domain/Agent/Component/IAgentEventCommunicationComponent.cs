using System.Threading.Tasks;
using AISmart.Events;
using Volo.Abp.EventBus;

namespace AISmart.AgentCommunicationComponent;

public interface IAgentEventCommunicationComponent : ILocalEventHandler<EventCreateEvent>,
    ILocalEventHandler<EventDispathchEvent>, ILocalEventHandler<EventFinishEvent>
{
}