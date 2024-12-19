using System;
using System.ComponentModel;
using AISmart.Agents;
using Orleans;

namespace AISmart.Events;
[Description("Represents an event corresponding to receiving a message within a chat.")]
[GenerateSerializer]
public class PumpFunReceiveMessageEvent : EventBase
{
    [Description("The replyId of request message.")]
    [Id(0)]  public string? ReplyId { get; set; }
    
    [Description("Text content of the received message.")]
    [Id(1)] public string? RequestMessage { get; set; }
    
}