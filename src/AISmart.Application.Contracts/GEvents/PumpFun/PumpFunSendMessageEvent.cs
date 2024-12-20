using System.ComponentModel;
using AISmart.Agents;
using Orleans;

namespace AISmart.Events;

[Description("Represents an event to send a PumpFun message within a chat.")]
[GenerateSerializer]
public class PumpFunSendMessageEvent : EventBase
{
    [Description("Unique identifier for the target chat where the ReplyMessage will be sent.")]
    [Id(0)]  
    public string? ChatId { get; set; }
    
    [Description("Optional ID of the ReplyMessage to which this message is a reply.")]
    [Id(1)]
    public string? ReplyId { get; set; }

    [Description("Text content of the ReplyMessage to be sent.")]
    [Id(2)]
    public string? ReplyMessage { get; set; }
}