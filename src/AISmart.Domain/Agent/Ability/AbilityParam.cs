using System;
using System.Collections.Generic;

namespace AISmart.Agent.Ability;

public class AbilityParam
{
    public Guid AgentId { get; set; }
    
    public string AbilityName { get; set; }
    
    public List<AbilityParamValue> ParamValues { get; set; }
}

public class AbilityParamValue
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string Value { get; set; }
}