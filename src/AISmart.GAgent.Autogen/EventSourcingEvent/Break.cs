namespace AISmart.GAgent.Autogen.EventSourcingEvent;

[GenerateSerializer]
public class Break:AutogenEventBase
{
    [Id(0)] public string BreakReason { get; set; }    
}