namespace AISmart.Domain.Grains.Event;

public class TaskDto
{
    public Guid Id { get; set; }
    
    public List<Guid> ProcessingEvents { get; set; }
    
    public List<Guid> FailedEvents { get; set; }
    
    public Dictionary<Guid,EventResult> EventResultDictionary { get; set; }
}

