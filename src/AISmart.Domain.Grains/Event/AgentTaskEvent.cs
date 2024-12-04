using Volo.Abp.Domain.Entities;

namespace AISmart.Domain.Grains.Event;


[GenerateSerializer]
public abstract class AgentTaskEvent
{
    [Id(0)]  public Guid Id { get; set; } = Guid.NewGuid();
    
    [Id(1)]   public Guid TemplateId { get; set; } = Guid.NewGuid();
    [Id(2)] public Guid TaskId { get; set; }
    [Id(3)] public string Name { get; set; }
}
[GenerateSerializer]
public  class CreatedAgentEvent : AgentTaskEvent
{
    [Id(4)] public bool IsCompleted { get; set; } = false;
    [Id(5)] public string Param { get; set; } 
}

[GenerateSerializer]
public  class CompletedAgentEvent : AgentTaskEvent
{
    [Id(6)] public Guid  CreatedEventId{ get; set; }
    [Id(7)] public bool IsSuccess { get; set; }
    [Id(8)] public string FailReason { get; set; }
    [Id(9)] public string Result { get; set; }
}



