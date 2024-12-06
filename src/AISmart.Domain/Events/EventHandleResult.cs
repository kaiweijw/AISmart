using System;
using AISmart.Agent.Ability;

namespace AISmart.Events;

public class EventHandleResult
{
    public Guid AgentId { get; set; }
    public Guid EventId { get; set; }
    public AbilityParam AbilityParam { get; set; }
    public bool IfSuccess { get; set; }
}