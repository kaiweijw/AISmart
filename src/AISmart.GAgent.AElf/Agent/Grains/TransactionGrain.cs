using System;
using System.Threading;
using System.Threading.Tasks;
using AElf.Client.MultiToken;
using AISmart.Agent.Event;
using AISmart.Provider;
using AISmart.Sender;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers;
using TransactionDto = AISmart.Dto.TransactionDto;

namespace AISmart.Agent.Grains;

[StorageProvider(ProviderName = "PubSubStore")]
public class TransactionGrain : Grain<AElfTransactionState>, ITransactionGrain
{
    public readonly IAElfNodeProvider _AElfNodeProvider;
    private ILogger<TransactionGrain> _Logger;
    public TransactionGrain(IAElfNodeProvider AElfNodeProvider,ILogger<TransactionGrain> Logger) 
    {
        _AElfNodeProvider = AElfNodeProvider;
        _Logger = Logger;
    }

    public async Task<TransactionDto> SendAElfTransactionAsync(SendTransactionDto sendTransactionDto)
    {
        var transaction = await _AElfNodeProvider.CreateTransactionAsync(sendTransactionDto.ChainId, sendTransactionDto.SenderName, sendTransactionDto.ContractAddress,
            sendTransactionDto.MethodName, new TransferInput());
        var sendTransactionAsync =  await _AElfNodeProvider.SendTransactionAsync(sendTransactionDto.ChainId,transaction);
        var publishingAgent = GrainFactory.GetGrain<IPublishingAgent>(Guid.NewGuid());
        await publishingAgent.PublishEventAsync(new SendTransactionCallBackEvent
        {
            TransactionId = sendTransactionAsync.TransactionId,
            
        });
        return new TransactionDto
        {
            TransactionId = sendTransactionAsync.TransactionId
        };
    }

    public async Task<TransactionDto> LoadAElfTransactionResultAsync(QueryTransactionDto queryTransactionDto)
    {
        bool isSuccess = false;
        TimeSpan timeout = TimeSpan.FromSeconds(30); 
        CancellationTokenSource cts = new CancellationTokenSource(timeout);
        try
        {
            while (true)
            {
                await Task.Delay(500, cts.Token);
                var txResult =
                    await _AElfNodeProvider.GetTransactionResultAsync(queryTransactionDto.ChainId,
                        queryTransactionDto.TransactionId);
                if (txResult.Status == "Mined")
                {
                    isSuccess = true;
                }

                if (txResult?.Status != "NOTEXISTED" && txResult?.Status != "PENDING") break;

            }
        }
        catch (OperationCanceledException e)
        {
            _Logger.LogError(e,"Transaction query timed out.");
        }
        var publishingAgent = GrainFactory.GetGrain<IPublishingAgent>(Guid.NewGuid());
        await publishingAgent.PublishEventAsync(new QueryTransactionCallBackEvent()
                {
                    TransactionId =  queryTransactionDto.TransactionId
                });
        return new TransactionDto
        {
            IsSuccess = isSuccess,
            TransactionId = queryTransactionDto.TransactionId
        };
    }

    public async Task <TransactionDto> GetAElfTransactionAsync(QueryTransactionDto queryTransactionDto)
    {
        return new TransactionDto
        {
            IsSuccess = State.IsSuccess,
            TransactionId = State.TransactionId
        };
    }
}