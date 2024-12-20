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
           // TODO:jim
           Id = Guid.Parse(@event.ReplyId),
           ChatId = @event.ChatId,
           ReplyId = @event.ReplyId,
           RequestMessage = @event.RequestMessage
       });
       await ConfirmEvents();
       
       await PublishAsync(new AutoGenCreatedEvent
       {
           // TODO:EventId?ReplyId?
           EventId = Guid.Parse(@event.ReplyId),
           Content = $"""
             Received a JSON-formatted message:{JsonConvert.SerializeObject(@event)}, The fields will be used in the final response expect "Message".
             Please follow the process below.
             1. parse the message content,the fields in the JSON may be used in the final response..
             2. Please understand the content of the "Message" in the JSON format, process the response accordingly.
             3. Must pass the final result to the PumpFunSendMessageEvent method of the PumpFunGAgent.
             """
       });
    }
    
    [EventHandler]
    public async Task HandleEventAsync(PumpFunSendMessageEvent @event)
    {
        if (@event.ReplyId != null)
        {
            RaiseEvent(new PumpFunSendMessageGEvent()
            {
                // TODO:jim
                Id = Guid.Parse(@event.ReplyId),
                ChatId = @event.ChatId,
                ReplyId = @event.ReplyId,
                ReplyMessage = @event.ReplyMessage
            });
            await ConfirmEvents();

            // TODO:jim Guid.Parse(@event.ReplyId)
            await GrainFactory.GetGrain<IPumFunGrain>(Guid.Parse(@event.ReplyId))
                .SendMessageAsync(@event.ReplyId, @event.ReplyMessage);
        }
    }
    
    public async Task SetPumpFunConfig(string chatId, string botName)
    {
        RaiseEvent(new SetPumpFunConfigEvent()
        {
            ChatId = chatId,
            BotName = botName
        });
        await ConfirmEvents();
    }

    public Task ExecuteTransactionAsync(PumpFunReceiveMessageGEvent gEventData)
    {
        throw new NotImplementedException();
    }

    public Task<PumpFunGAgentState> GetPumpFunGAgentState()
    {
        PumpFunGAgentState pumpFunGAgentState = new PumpFunGAgentState();
        pumpFunGAgentState.Id = State.Id;
        pumpFunGAgentState.ChatId = State.ChatId;
        pumpFunGAgentState.BotName = State.BotName;
        return Task.FromResult(pumpFunGAgentState);
    }
}

public interface IPumpFunGAgent : IStateGAgent<PumpFunGAgentState>
{ 
    Task SetPumpFunConfig(string chatId, string botName);
    
    Task ExecuteTransactionAsync(PumpFunReceiveMessageGEvent gEventData);
    
    Task<PumpFunGAgentState> GetPumpFunGAgentState();
    
}