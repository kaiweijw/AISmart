using System;
using System.ComponentModel;
using AISmart.Agents;
using Orleans;

namespace AISmart.Events;
[Description("Represents an event corresponding to receiving a message within a chat.")]
[GenerateSerializer]
public class PumpFunReceiveMessageEvent : EventBase
{
    [Description("Unique identifier for the chat from which the message was received.")]
    [Id(0)]  public string? ChatId { get; set; }
    
    [Description("The replyId of request message.")]
    [Id(1)]  public string? ReplyId { get; set; }
    
    [Description("Text content of the received message.")]
    [Id(2)] public string? RequestMessage { get; set; }
    
}