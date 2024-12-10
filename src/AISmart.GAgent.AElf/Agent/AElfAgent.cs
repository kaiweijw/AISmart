using System.Threading.Tasks;
using AElf.Contracts.MultiToken;
using AISmart.Agent.Event;
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
public class AElfAgent : GAgent<AElfTransactionState, CreateTransactionEvent>,IAElfAgent
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

    public async Task<TransactionDto> GetAElfTransactionAsync()
    {
        return new TransactionDto
        {
            IsSuccess = State.IsSuccess,
            TransactionId = State.TransactionId
        };
    }

    public async Task ExecuteTransactionAsync(CreateTransactionEvent eventData)
    {
         await ExecuteAsync( eventData);
    }

    protected override async Task ExecuteAsync(CreateTransactionEvent eventData)
    {
        var transaction = await _AElfNodeProvider.CreateTransactionAsync(eventData.ChainId, eventData.SenderName, eventData.ContractAddress,
            eventData.MethodName, new TransferInput());
        var SendTransactionOutput =  await _AElfNodeProvider.SendTransactionAsync(eventData.ChainId,transaction);
       
        Logger.LogInformation("ExecuteAsync: AElf {MethodName}", eventData.MethodName);
        var transactionResult = await _AElfNodeProvider.GetTransactionResultAsync(eventData.ChainId,SendTransactionOutput.TransactionId);
        if (transactionResult.Status == "Mined")
        {
            eventData.IsSuccess = true;
            eventData.TransactionId = SendTransactionOutput.TransactionId;
            base.RaiseEvent(eventData);
            await PublishAsync(eventData);
        }
    }

    protected override void TransitionState(AElfTransactionState state, CreateTransactionEvent @event)
    {
        switch (@event)
        {
           
            case CreateTransactionEvent createEvent:
                State.ChainId = createEvent.ChainId;
                State.SenderName = createEvent.SenderName;
                State.MethodName = createEvent.MethodName;
                State.ContractAddress = createEvent.ContractAddress;
                State.Param = createEvent.Param;
                if (createEvent.IsSuccess)
                {
                    State.TransactionId = createEvent.TransactionId;
                }
                State.IsSuccess = createEvent.IsSuccess;
                break;
            default:
                break;
        }
    }
}

public interface IAElfAgent : IGrainWithGuidKey
{ 
    Task <TransactionDto> GetAElfTransactionAsync();
    Task ExecuteTransactionAsync(CreateTransactionEvent eventData);
}