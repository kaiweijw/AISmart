using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Contracts.MultiToken;
using AElf.Types;
using AISmart.Provider;
using Google.Protobuf;
using Xunit.Abstractions;

namespace AISmart.Mock;

public class MockAElfNodeProvider: IAElfNodeProvider
{
    private readonly ITestOutputHelper _output;

    public MockAElfNodeProvider(ITestOutputHelper output)
    {
        _output = output;
    }

    public Task<Transaction> CreateTransactionAsync(string chainId, string senderName, string contractAddress, string methodName,
        IMessage param)
    {
        return Task.FromResult(new Transaction
        {
            From = null,
            To = null,
            RefBlockNumber = 0,
            MethodName = methodName,
            Params = null,
            Signature = null
        });
        
    }

    public Task<SendTransactionOutput> SendTransactionAsync(string chainId, Transaction transaction)
    {
         _output.WriteLine("SendTransaction success methodName: " + transaction.MethodName +
                                 "TransactionId: e1c3bb3be887e2b202d1611b797eff885b25d8c03688adfd5b8a9603cb4b9858");
         return Task.FromResult(new SendTransactionOutput
         {
             TransactionId = "e1c3bb3be887e2b202d1611b797eff885b25d8c03688adfd5b8a9603cb4b9858"
         });
    }

    public Task<TransactionResultDto> GetTransactionResultAsync(string chainId, string transactionId)
    {
        return Task.FromResult(new TransactionResultDto
        {
            TransactionId = transactionId,
            Status = "Mined",
            Logs = new LogEventDto[]
            {
            },
            BlockNumber = 100,
            BlockHash = "",
            Transaction = new TransactionDto
            {
                From = "From",
                To = "To",
                RefBlockNumber = 100,
                MethodName = "Transfer"
               
            },
        });
    }

    public Task<T> CallTransactionAsync<T>(string chainId, Transaction transaction) where T : class, IMessage<T>, new()
    {
        throw new System.NotImplementedException();
    }
}