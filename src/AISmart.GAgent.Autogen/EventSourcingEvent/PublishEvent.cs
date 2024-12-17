namespace AISmart.GAgent.Autogen.EventSourcingEvent;

public class PublishEvent : AutogenEventBase
{
    [Id(0)] public Guid EventId { get; set; }
}