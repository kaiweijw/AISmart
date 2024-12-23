using Orleans;

namespace AISmart.Agents.A.Events;

[GenerateSerializer]
public class AThreadCreatedEvent : EventBase
{
    [Id(0)] public string Id { get; set; }
    [Id(1)] public string Content { get; set; }
}