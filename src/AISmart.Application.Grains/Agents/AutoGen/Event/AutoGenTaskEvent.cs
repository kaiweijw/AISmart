namespace AISmart.Application.Grains.Agents.AutoGen.Event;

public class AutoGenTaskEvent
{
    public TaskExecuteStatus ExecuteStatus { get; set; }
    public string EventName { get; set; }
    public string EventResponse { get; set; }
    public string BreakReason { get; set; }
}

public enum TaskExecuteStatus
{
    Break,
    Finish,
    Progress,
}