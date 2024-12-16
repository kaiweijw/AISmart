using Volo.Abp.EventBus.Local;

namespace AISmart.Application.Grains.Command;

public class EventBusService : IEventBusService
{
    private readonly ILocalEventBus _eventBus;

    public EventBusService(ILocalEventBus eventBus)
    { 
        _eventBus = eventBus;
    }
    public ILocalEventBus GetEventBus()
    {
        return _eventBus;
    }
}