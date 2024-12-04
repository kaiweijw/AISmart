namespace AISmart.Domain.Grains.Event;

public class TaskState
{
    public Guid Id { get; set; }
    
    public string State { get; set; }
    
    public List<Guid> ProcessingEvents { get; set; }
    
    public List<Guid> FailedEvents { get; set; }
    
    public Dictionary<Guid,EventResult> EventResultDictionary { get; set; }
}

public class EventResult
{
    public string AgentTopic { get; set; }
    public string Result { get; set; }
}

