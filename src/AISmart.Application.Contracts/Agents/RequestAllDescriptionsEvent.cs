using System;
using System.ComponentModel.DataAnnotations;
using Orleans;

namespace AISmart.Agents;

[GenerateSerializer]
public class RequestAllSubscriptionsEvent : EventWithResponseBase<SubscribedEventListEvent>
{
    [Id(0)][Required] public Type RequestFromGAgentType { get; set; }
}