using Orleans;

namespace AISmart.Agents.ImplementationAgent.Events;

[GenerateSerializer]
public class ImplementationEvent:BaseEvent
{
    [Id(0)] public string Content { get; set; }
}