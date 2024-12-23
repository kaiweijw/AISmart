using System.Collections.Generic;
using Orleans;

namespace AISmart.Agents.C;

[GenerateSerializer]
public class CAgentState : StateBase
{
    [Id(0)] public List<string> ThreadIds { get; set; }
    [Id(1)] public int Number { get; set; }
}