using System;
using Google.Protobuf;
using Orleans;

namespace AISmart.Agent.Event;

[GenerateSerializer]
public  class SendTransactionGEvent: CreateTransactionGEvent
{
    [Id(0)] public Guid Id { get; set; }
   
    [Id(6)] public bool IsSuccess   { get; set; }
    
    [Id(7)] public string TransactionId { get; set; }
}