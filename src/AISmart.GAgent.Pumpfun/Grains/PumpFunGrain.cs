using System.Threading.Tasks;
using AISmart.Dto;
using AISmart.Grains;
using AISmart.Provider;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.Providers;

namespace AISmart.Agent.Grains;

[StorageProvider(ProviderName = "PubSubStore")]
public class PumpFunGrain : Grain<PumpFunState>, IPumFunGrain
{
    public readonly IPumpFunProvider _pumpFunProvider;
    public PumpFunGrain(IPumpFunProvider pumpFunProvider) 
    {
        _pumpFunProvider = pumpFunProvider;
    }

    public async Task SendMessageAsync(string replyId, string? replyMessage)
    {
        if (replyMessage != null)
        {
            await _pumpFunProvider.SendMessageAsync(replyId, replyMessage);
        }
    }
}