using System.Collections.Generic;

namespace AISmart.Options;

public class AgentNetworkConfigOptions
{
    public List<Contract> ContractsList { get; set; }
    public List<Group> Groups { get; set; }
}

public class Contract
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string AgentState { get; set; }
}

public class Group
{
    public List<string> AgentsList { get; set; }
    public List<Relation> RelationList { get; set; }
}

public class Relation
{
    public string From { get; set; }
    public string To { get; set; }
}