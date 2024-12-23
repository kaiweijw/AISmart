using Orleans;

namespace AISmart.Agents.B.Events;

[GenerateSerializer]
public class BThreadCreatedEvent : EventBase
{
    [Id(0)] public string Id { get; set; }
    [Id(1)] public string Content { get; set; }
}