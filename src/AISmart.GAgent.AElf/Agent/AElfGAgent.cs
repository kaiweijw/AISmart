using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Agent.Event;
using AISmart.Agent.GEvents;
using AISmart.Agent.Grains;
using AISmart.Application.Grains;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;

namespace AISmart.Agent;
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class AElfGAgent(ILogger<AElfGAgent> logger) : GAgent<AElfAgentGState, TransactionGEvent>(logger), IAElfAgent
{
    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a aelf thread is published.");
    }


    protected  Task ExecuteAsync(CreateTransactionGEvent gEventData)
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
        Logger.LogInformation("ExecuteAsync: AElf {MethodName}", gEventData.MethodName);
        return Task.CompletedTask;
    }
    
    protected  Task ExecuteAsync(SendTransactionCallBackSEvent gEventData)
    {
        base.RaiseEvent(new SendTransactionGEvent
        {
            CreateTransactionGEventId = gEventData.CreateTransactionGEventId,
            ChainId = gEventData.ChainId,
            TransactionId = gEventData.TransactionId
        });
       
        _= GrainFactory.GetGrain<ITransactionGrain>(gEventData.Id).LoadAElfTransactionResultAsync(
            new QueryTransactionDto
            {
                CreateTransactionGEventId = gEventData.CreateTransactionGEventId,
                ChainId = gEventData.ChainId,
                TransactionId = gEventData.TransactionId
            });
        return Task.CompletedTask;
    }

   

    protected async Task ExecuteAsync(QueryTransactionCallBackSEvent gEventData)
    {
        if (gEventData.IsSuccess)
        {
            base.RaiseEvent(new TransactionSuccessGEvent
            {
                CreateTransactionGEventId = gEventData.CreateTransactionGEventId
            });
        }
        else
        {
            base.RaiseEvent(new TransactionFailedGEvent()
            {
                CreateTransactionGEventId = gEventData.CreateTransactionGEventId,
                Error = gEventData.Error
            });
        }
        await ConfirmEvents();
    }


    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
         await base.OnActivateAsync(cancellationToken);
         await SubscribeAsync<CreateTransactionGEvent>(ExecuteAsync);
         await SubscribeAsync<SendTransactionGEvent>(ExecuteAsync);
         await SubscribeAsync<SendTransactionCallBackSEvent>(ExecuteAsync);
         await SubscribeAsync<QueryTransactionCallBackSEvent>(ExecuteAsync);
    }

    public async Task ExecuteTransactionAsync(CreateTransactionGEvent gEventData)
    {
        await ExecuteAsync( gEventData);
    }


    protected override Task ExecuteAsync(TransactionGEvent eventData)
    {
        throw new NotImplementedException();
    }

    protected override Task CompleteAsync(TransactionGEvent eventData)
    {
        throw new NotImplementedException();
    }
}

public interface IAElfAgent : IGrainWithGuidKey
{ 
    Task ExecuteTransactionAsync(CreateTransactionGEvent gEventData);
}