using Orleans;

namespace AISmart.Agent.GEvents;
[GenerateSerializer]
public class SendMessageGEvent : MessageGEvent
{
    [Id(1)] public string ChatId { get; set; }
    [Id(2)] public string MessageId { get; set; }
    [Id(3)] public string Message { get; set; } //text or photo
}