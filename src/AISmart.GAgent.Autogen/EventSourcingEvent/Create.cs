using AutoGen.Core;

namespace AISmart.GAgent.Autogen.EventSourcingEvent;

[GenerateSerializer]
public class Create:AutogenEventBase
{
    [Id(0)] public List<IMessage> Messages { get; set; } = new List<IMessage>();
    [Id(1)] public long CreateTime { get; set; }
}