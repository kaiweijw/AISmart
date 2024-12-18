using AISmart.Agents.ImplementationAgent.Events;
using Orleans;

namespace AISmart.Agents.MarketLeader.Events;

[GenerateSerializer]
public class SocialEvent : EventWithResponseBase<ImplementationEvent>
{
    [Id(0)] public string Content { get; set; }
}