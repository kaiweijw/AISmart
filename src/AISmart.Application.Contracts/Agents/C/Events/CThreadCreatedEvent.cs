using Orleans;

namespace AISmart.Agents.C.Events;

[GenerateSerializer]
public class CThreadCreatedEvent : EventBase
{
    [Id(0)] public string Id { get; set; }
    [Id(1)] public string Content { get; set; }
}