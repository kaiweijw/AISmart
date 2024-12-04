using Volo.Abp.Domain.Entities;

namespace AISmart.Domain.Grains.Event;



public abstract class TaskEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid TemplateId { get; set; } = Guid.NewGuid();
    public Guid TaskId { get; set; }
    public string Name { get; set; }
}

public  class CreatedEvent : TaskEvent
{
    public bool IsCompleted { get; set; } = false;
    public string Param { get; set; } 
}

public  class CompletedTaskEvent : TaskEvent
{
    public Guid  CreatedEventId{ get; set; }
    public bool IsSuccess { get; set; }
    public string FailReason { get; set; }
    public string Result { get; set; }
}



