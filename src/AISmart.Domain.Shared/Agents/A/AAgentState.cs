using System.Collections.Generic;
using Orleans;

namespace AISmart.Agents.A;

[GenerateSerializer]
public class AAgentState : StateBase
{
    [Id(0)] public List<string> ThreadIds { get; set; }
    [Id(1)] public int Number { get; set; }
}