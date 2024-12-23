using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.A.Events;
using AISmart.Agents.C;
using AISmart.Agents.C.Events;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.C;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class CGAgent : GAgentBase<CAgentState, CGEvent>
{
    public CGAgent(ILogger<CGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a A thread is published.");
    }

    private Task TryExecuteAsync(CThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: A Thread {AContent}", eventData.Content);
        return Task.CompletedTask;
    }

    [EventHandler]
    public async Task HandleEventAsync(CThreadCreatedEvent eventData)
    {
        Console.WriteLine($"{GetType()} ExecuteAsync: CAgent analyses content: {eventData.Content}");

        for (int i = 0; i < 100; i++)
        {
            State.Number += 1;
        }
    }
}