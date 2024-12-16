using Volo.Abp.EventBus.Local;

namespace AISmart.Application.Grains.Command;

public interface IEventBusService
{
    public ILocalEventBus GetEventBus();
}