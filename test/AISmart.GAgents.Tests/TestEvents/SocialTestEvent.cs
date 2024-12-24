using AISmart.Agents;

namespace AISmart.GAgents.Tests.TestEvents;

[GenerateSerializer]
public class SocialTestEvent : EventBase
{
    [Id(0)] public string MessageId { get; set; }
    [Id(1)] public string Message { get; set; }
}