using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Agent.GEvents;
using AISmart.Agents;
using AISmart.Agents.AutoGen;
using AISmart.Application.Grains;
using AISmart.Events;
using AISmart.Grains;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans.Providers;

namespace AISmart.Agent;

[Description("Handle PumpFun")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class PumpFunGAgent : GAgentBase<PumpFunGAgentState, PumpFunMessageGEvent>, IPumpFunGAgent
{
    private readonly ILogger<PumpFunGAgent> _logger;
    public PumpFunGAgent(ILogger<PumpFunGAgent> logger) : base(logger)
    {
        _logger = logger;
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Represents an agent responsible for informing other agents when a PumpFun thread is published.");
    }
    
    // TODO:这个方法需要吗？
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
        // await SubscribeAsync<ReceiveMessageEvent>(ExecuteAsync);
        // await SubscribeAsync<SendMessageEvent>(ExecuteAsync);
    }

    [EventHandler]
    public async Task HandleEventAsync(PumpFunReceiveMessageEvent @event)
    { 
        _logger.LogInformation("PumpFunReceiveMessageEvent:" + JsonConvert.SerializeObject(@event));
       RaiseEvent(new PumpFunReceiveMessageGEvent
       {
           // TODO:messageId&ChatId needed?
           ReplyId = @event.ReplyId,
           // ChatId = @event.ChatId,
           RequestMessage = @event.RequestMessage
       });
       await ConfirmEvents();
       if (@event.ReplyId != null)
       {
           await PublishAsync(new AutoGenCreatedEvent
           {
               // TODO:EventId?ReplyId?
               EventId = Guid.Parse(@event.ReplyId),
               Content = $"I received a JSON-formatted message:{JsonConvert.SerializeObject(@event)}. " +
                         $"Please parse the message content, generate a response Based on the JSON Message, " +
                         $"and then call the SendMessageEvent event of PumpFunGAgent"
           });
       }
    }
    
    [EventHandler]
    public async Task HandleEventAsync(PumpFunSendMessageEvent @event)
    {
        RaiseEvent(new PumpFunSendMessageGEvent()
        {
            ReplyId = @event.ReplyId,
            // TODO
            // ChatId = @event.ChatId,
            ReplyMessage = @event.ReplyMessage 
        });
        await ConfirmEvents();
        
        await GrainFactory.GetGrain<IPumFunGrain>(Guid.NewGuid())
            .SendMessageAsync(@event.ReplyId, @event.ReplyMessage);
    }

    
}

public interface IPumpFunGAgent
{ 
   
}