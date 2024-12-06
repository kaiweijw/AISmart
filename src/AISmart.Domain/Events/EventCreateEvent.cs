using Volo.Abp.EventBus;

namespace AISmart.Events;

public class EventCreateEvent
{
    public  Event Event { get; set; }
}