using Orleans;

namespace AISmart.Agents.X.Events;

[GenerateSerializer]
public class XThreadCreatedEvent : GEvent
{
    /// <summary>
    /// 
    /// </summary>
    [Id(0)] public string Id { get; set; }
    
    /// <summary>
    /// aboat aelf chain all inform
    /// </summary>
    [Id(1)] public string Content { get; set; }

    public string contentt;
}