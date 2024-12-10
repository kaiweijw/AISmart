namespace AISmart.Application.Grains.Agents.X.Events;

[GenerateSerializer]
public class XThreadCreatedEvent
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string Content { get; set; }
}