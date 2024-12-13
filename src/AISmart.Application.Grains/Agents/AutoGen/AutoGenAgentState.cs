using AISmart.Agents.AutoGen;
using AutoGen.Core;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.AutoGen;

[GenerateSerializer]
public class AutoGenAgentState
{
    [Id(0)] public Dictionary<Guid, AutoGenAgentStateInfo> AutoGenStateDic =
        new Dictionary<Guid, AutoGenAgentStateInfo>();
}

public class AutoGenAgentStateInfo
{
    [Id(0)] public Guid TaskId { get; set; }
    [Id(1)] public List<IMessage> ChatHistory { get; set; }
    [Id(2)] public long SessionStartTime { get; set; }
    [Id(3)] public SessionStateEnum SessionStateEnum { get; set; }
    [Id(4)] public string CurrentCall { get; set; }
}

[GenerateSerializer]
public enum SessionStateEnum
{
    Processing,
    Break,
    Completed
}
