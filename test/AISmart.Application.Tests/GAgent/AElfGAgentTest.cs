using System.Threading.Tasks;
using AElf.Client.Dto;
using AElf.Contracts.MultiToken;
using AElf.Types;
using AISmart.Provider;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.GAgent;

public class AElfGAgentTests : AISmartApplicationTestBase
{
    private readonly IAElfNodeProvider _aelfNodeProvider;
    private readonly ITestOutputHelper _output;
    public AElfGAgentTests(ITestOutputHelper output)
    {
        _aelfNodeProvider = GetRequiredService<IAElfNodeProvider>();
        _output = output;
    }

    [Fact]
    public async Task SendTransactionTest()
    {
        string chainId = "AELF";
        string senderName = "Test";
        var transaction =  await  _aelfNodeProvider.CreateTransactionAsync(chainId, senderName, "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE", "Transfer",
            new TransferInput
            {
                To = Address.FromBase58("2KQWh5v6Y24VcGgsx2KHpQvRyyU5DnCZ4eAUPqGQbnuZgExKaV"),
                Symbol = "ELF",
                Amount = 10
            });
        var sendTransactionOutput = await _aelfNodeProvider.SendTransactionAsync(chainId, transaction);
        _output.WriteLine("TransactionId: " + sendTransactionOutput.TransactionId);
        await Task.Delay(3000);
        var resultAsync = await _aelfNodeProvider.GetTransactionResultAsync(chainId,sendTransactionOutput.TransactionId);
        _output.WriteLine("resultAsync status: " + resultAsync.Status);
    }
    
    [Fact]
    public async Task CallTransactionTest()
    {
        string chainId = "AELF";
        string senderName = "Test";
        var transaction =  await  _aelfNodeProvider.CreateTransactionAsync(chainId, senderName, "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE", "GetBalance",
            new GetBalanceInput
            {
                Symbol = "ELF",
                Owner = Address.FromBase58("2KQWh5v6Y24VcGgsx2KHpQvRyyU5DnCZ4eAUPqGQbnuZgExKaV")
            });
        var GetBalanceOutput = await _aelfNodeProvider.CallTransactionAsync<GetBalanceOutput>(chainId, transaction);
        _output.WriteLine("Balance: " + GetBalanceOutput.Balance);
    }

}