using System;

namespace AISmart.Events;

public class EventDispatchModel
{
    public Guid EventId { get; set; }
    
    public Guid AgentId { get; set; }
}