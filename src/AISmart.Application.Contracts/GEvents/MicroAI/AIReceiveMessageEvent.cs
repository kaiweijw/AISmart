using System;
using System.ComponentModel;
using AISmart.Agents;
using AISmart.GEvents.MicroAI;
using Orleans;

namespace AISmart.Events;
[Description("Receive a message from telegram.")]
[GenerateSerializer]
public class AIReceiveMessageEvent :  EventWithResponseBase<AIResponseEvent>
{
    [Description("Unique identifier for the received message.")]
    [Id(0)]  public Guid MessageId { get; set; }
    [Description("Unique identifier for the chat from which the message was received.")]
    [Id(1)] public string Message { get; set; }
   
}