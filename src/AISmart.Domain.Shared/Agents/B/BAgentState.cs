using System.Collections.Generic;
using Orleans;

namespace AISmart.Agents.B;

[GenerateSerializer]
public class BAgentState : StateBase
{
    [Id(0)] public List<string> ThreadIds { get; set; }
    [Id(1)] public int Number { get; set; }
}