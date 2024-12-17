using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agent.Events;
using Orleans;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.GAgent;

public class CqrsTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ITestOutputHelper _output;
    public CqrsTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        _output = output;
    }

    [Fact]
    public async Task SendTransactionTest()
    {
        string chainId = "AELF";
        string senderName = "Test";
        var createTransactionEvent = new CreateTransactionEvent(){
                ChainId = chainId,
                SenderName = senderName, 
                ContractAddress = "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE",
                MethodName = "Transfer",
                };
                var guid = Guid.NewGuid();
                await _clusterClient.GetGrain<IAElfAgent>(guid).ExecuteTransactionAsync(createTransactionEvent);
               var transaction = await _clusterClient.GetGrain<IAElfAgent>(guid).GetAElfAgentDto();
                _output.WriteLine("TransactionId: " + transaction.PendingTransactions.Count);
    }
    

}