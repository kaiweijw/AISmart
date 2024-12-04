namespace AISmart.Domain.Grains.Event;

public class EventFlowTemplateState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Description { get; set; }
    public string AgentTopic { get; set; }
    public Guid? Upstream { get; set; }
    public List<Guid> Downstreams { get; set; } = new List<Guid>();
}
