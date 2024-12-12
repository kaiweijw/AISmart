using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AElf.Client.MultiToken;
using AISmart.Agent.Event;
using AISmart.Agent.Grains;
using AISmart.Dto;
using AISmart.Provider;
using Microsoft.Extensions.Logging;
using Orleans;

namespace AISmart.Agent.Grains;

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
        //subscribe SendTransactionSEvent
        var transaction = await _AElfNodeProvider.CreateTransactionAsync(sendTransactionDto.ChainId, sendTransactionDto.SenderName, sendTransactionDto.ContractAddress,
            sendTransactionDto.MethodName, new TransferInput());
        var sendTransactionAsync =  await _AElfNodeProvider.SendTransactionAsync(sendTransactionDto.ChainId,transaction);
        return new TransactionDto
        {
            TransactionId = sendTransactionAsync.TransactionId
        };
        // publish SendTransactionCallBackSEvent
    }

    public async Task<TransactionDto> LoadAElfTransactionResultAsync(QueryTransactionDto queryTransactionDto)
    {
        //subscribe SendTransactionSEvent
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

        // publish QueryTransactionCallBackSEvent
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