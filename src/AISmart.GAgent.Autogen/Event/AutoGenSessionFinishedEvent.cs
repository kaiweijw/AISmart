using AISmart.Agents;

namespace AISmart.GAgent.Autogen.Event;

public class AutoGenSessionFinishedEvent : EventBase
{
    public Guid TaskId { get; set; }
    public TaskExecuteStatus ExecuteStatus { get; set; }
    public string EndContent { get; set; }
}

public enum TaskExecuteStatus
{
    Break,
    Finish,
}