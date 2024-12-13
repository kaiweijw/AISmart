namespace AISmart.GAgent.Autogen.EventSourcingEvent;

[GenerateSerializer]
public class BaseEvent:Agents.GEvent
{
    [Id(0)] public Guid Id { get; set; }
}