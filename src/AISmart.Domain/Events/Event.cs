using System;
using System.Collections.Generic;
using AISmart.Agent.Coordinator;

namespace AISmart.Events;

public class Event
{
    public Guid EventId { get; set; }
    public string OriginalContent { get; set; }
    public List<AgentFlow> AgentFlows { get; set; }
}