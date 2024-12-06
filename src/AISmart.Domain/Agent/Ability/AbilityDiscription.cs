using System;
using System.Collections.Generic;

namespace AISmart.Agent.Ability;

public class AbilityDescription
{
    // public Guid AgentId { get; set; }
    public string AbilityName { get; set; }
    public string Description { get; set; }
    public List<AbilityDependence> AbilityDepends { get; set; }
}

public class AbilityDependence
{
    public bool IsRequired { get; set; }
    
    public string Name { get; set; }
    
    public string Description { get; set; }
    
    public string Type { get; set; }
}
