using System;
using System.Collections.Generic;

namespace AISmart.Agent.Coordinator;

public class CoordinatorResult
{
    public List<AgentFlow> AgentFlows { get; set; } = null;
}

public class AgentFlow
{
    public Guid AgentId { get; set; }

    public List<AgentFlow> AgentFlows { get; set; } = null;
}