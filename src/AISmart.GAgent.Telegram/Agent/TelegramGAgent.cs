using System;
using System.ComponentModel;
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

[Description("Represents an agent responsible for informing other agents when a Telegram thread is published.")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class TelegramGAgent : GAgentBase<TelegramGAgentState, MessageGEvent>, ITelegramGAgent
{
    private readonly ILogger<TelegramGAgent> _logger;
    public TelegramGAgent(ILogger<TelegramGAgent> logger) : base(logger)
    {
        _logger = logger;
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Represents an agent responsible for informing other agents when a Telegram thread is published.");
    }
    

    [EventHandler]
    public async Task ExecuteAsync(ReceiveMessageEvent @event)
    { 
        _logger.LogInformation("ReceiveMessageEvent "+@event.MessageId);
       RaiseEvent(new ReceiveMessageGEvent
       {
           MessageId = @event.MessageId,
           ChatId = @event.ChatId,
           Message = @event.Message
       });
       await ConfirmEvents();
       await PublishAsync(new AutoGenCreatedEvent
       {
           EventId = Guid.NewGuid(),
           Content = JsonConvert.SerializeObject(@event)+" Processes the message to generate an intelligent response"
       });


    }
    [EventHandler]
    public async Task ExecuteAsync(SendMessageEvent @event)
    {
        if (@event.ReplyMessageId != null)
        {
            RaiseEvent(new SendMessageGEvent()
            {
                ReplyMessageId = @event.ReplyMessageId,
                ChatId = @event.ChatId,
                Message = @event.Message.IsNullOrEmpty() ? 
                    @event.Photo : @event.Message
            });
            await ConfirmEvents();
        }
        await GrainFactory.GetGrain<ITelegramGrain>(Guid.NewGuid()).SendMessageAsync(
                @event.SenderBotName, @event.ChatId, @event.Message, @event.ReplyMessageId);
        
    }

    
}

public interface ITelegramGAgent
{ 
   
}