using System;
using AISmart.Agents;

namespace AISmart.GAgent.Autogen.Event;

public class AutoGenExecutorEvent : EventBase
{
    public Guid TaskId { get; set; }
    public TaskExecuteStatus ExecuteStatus { get; set; }
    public string CurrentCallInfo { get; set; }
    public string EndContent { get; set; }
}

public enum TaskExecuteStatus
{
    Progressing,
    Break,
    Finish,
}