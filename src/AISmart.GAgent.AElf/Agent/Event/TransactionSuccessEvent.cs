using System;
using Orleans;

namespace AISmart.Agent.Event;

public  class TransactionSuccessEvent : CreateTransactionEvent
{
    [Id(0)] public Guid Id { get; set; } = Guid.NewGuid();
    [Id(1)] public string TransactionId { get; set; }
    [Id(2)] public Guid CreateTransactionEventId { get; set; }
    [Id(2)] public Guid TaskId { get; set; }
}