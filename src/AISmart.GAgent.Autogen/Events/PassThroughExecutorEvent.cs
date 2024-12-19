using AISmart.GAgent.Autogen.Event;
using AISmart.GAgent.Autogen.Events;
using Orleans;

namespace AISmart.GEvents.Autogen;

[GenerateSerializer]
public class PassThroughExecutorEvent : AutoGenInternalEventBase
{
    [Id(0)] public Guid TaskId { get; set; }
    [Id(1)] public object PassThroughData { get; set; }
}