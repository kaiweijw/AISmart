using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AISmart.Agent.Event;
using AISmart.Agent.GEvent;
using AISmart.Agent.Grains;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Agents.X;
using AISmart.Application.Grains.Agents.X.Events;
using AISmart.Dto;
using AISmart.Provider;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;

namespace AISmart.Agent;

[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AElfAgent : GAgent<AElfAgentState, CreateTransactionGEvent>,IAElfAgent
{
    public readonly IAElfNodeProvider _AElfNodeProvider;
    public AElfAgent(ILogger<AElfAgent> logger,IAElfNodeProvider AElfNodeProvider) : base(logger)
    {
        _AElfNodeProvider = AElfNodeProvider;
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a aelf thread is published.");
    }

    public async Task ExecuteTransactionAsync(CreateTransactionGEvent gEventData)
    {
         await ExecuteAsync( gEventData);
    }
    
    protected override async Task ExecuteAsync(CreateTransactionGEvent gEventData)
    {

        base.RaiseEvent(gEventData);
        // await PublishAsync(eventData);
        Logger.LogInformation("ExecuteAsync: AElf {MethodName}", gEventData.MethodName);
    }
    
    protected  async Task ApplyAsync(CreateTransactionGEvent gEventData)
    {
        base.RaiseEvent(gEventData);
        _= GrainFactory.GetGrain<ITransactionGrain>(gEventData.Id).SendAElfTransactionAsync(
            new SendTransactionDto
            {
                Id = gEventData.Id,
                ChainId = gEventData.ChainId,
                SenderName = gEventData.SenderName,
                ContractAddress = gEventData.ContractAddress,
                MethodName = gEventData.MethodName,
                Param = gEventData.Param
            });
    }
    // from SendTransactionCallBackEvent
    protected  async Task ApplyAsync(SendTransactionGEvent gEventData)
    {
        base.RaiseEvent(gEventData);
        _= GrainFactory.GetGrain<ITransactionGrain>(gEventData.Id).LoadAElfTransactionResultAsync(
            new QueryTransactionDto
            {
                Id = gEventData.Id,
                ChainId = gEventData.ChainId,
                TransactionId = gEventData.TransactionId
            });
      
    }
    
    // from queryTransactionCallBackEvent
    protected  async Task ApplyAsync(TransactionSuccessGEvent successGEventData)
    {
        base.RaiseEvent(successGEventData);
        await ConfirmEvents();
    }
    // from queryTransactionCallBackEvent
    protected  async Task ApplyAsync(TransactionFailedGEvent gEventData)
    {
        base.RaiseEvent(gEventData);
        await ConfirmEvents();
    }

    protected override void TransitionState(AElfAgentState state, CreateTransactionGEvent gEvent)
    {
        switch (gEvent)
        {
            case TransactionSuccessGEvent transactionSuccessGEvent:
                state.PendingTransactions ??= new Dictionary<Guid, CreateTransactionGEvent>();
                state.PendingTransactions.Remove(transactionSuccessGEvent.Id);
                break;
            case TransactionFailedGEvent transactionFailedGEvent:
                state.PendingTransactions ??= new Dictionary<Guid, CreateTransactionGEvent>();
                state.PendingTransactions.Remove(transactionFailedGEvent.Id);
                break;
            case CreateTransactionGEvent createEvent:
                state.PendingTransactions ??= new Dictionary<Guid, CreateTransactionGEvent>();
                state.PendingTransactions[createEvent.Id] = createEvent;
                break;
            default:
                break;
        }
    }
}

public interface IAElfAgent : IGrainWithGuidKey
{ 
    Task ExecuteTransactionAsync(CreateTransactionGEvent gEventData);
}