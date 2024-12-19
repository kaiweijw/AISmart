using System.ComponentModel;
using AISmart.Agents;
using Orleans;

namespace AISmart.Events;

[Description("Represents an event to send a telegram message within a chat.")]
[GenerateSerializer]
public class PumpFunSendMessageEvent : EventBase
{
    [Description("Unique identifier for the target chat where the message will be sent.")]
    [Id(0)]
    public string ReplyId { get; set; }

    [Description("The answer of the message to be sent.")]
    [Id(1)]
    public string? ReplyMessage { get; set; }
}