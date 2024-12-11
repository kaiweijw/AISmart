using System;
using Google.Protobuf;
using Orleans;

namespace AISmart.Agent.Event;

[GenerateSerializer]
public  class QueryTransactionCallBackSEvent
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public string ChainId { get; set; }
    [Id(2)] public string TransactionId { get; set; }
    
    [Id(6)] public bool IsSuccess   { get; set; }
}