using System.Diagnostics.CodeAnalysis;
using AISmart.Agents;
using AISmart.Agents.A.Events;
using AISmart.Agents.C;
using AISmart.Agents.C.Events;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Application.Grains.Agents.MockC;

[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class MockCGAgent : GAgentBase<MockCAgentState, MockCGEvent>
{
    public MockCGAgent(ILogger<MockCGAgent> logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a A thread is published.");
    }

    private Task TryExecuteAsync(MockCThreadCreatedEvent eventData)
    {
        Logger.LogInformation("TryExecuteAsync: A Thread {AContent}", eventData.Content);
        return Task.CompletedTask;
    }

    [EventHandler]
    public async Task HandleEventAsync(MockCThreadCreatedEvent eventData)
    {
        Console.WriteLine($"{GetType()} ExecuteAsync: CAgent analyses content: {eventData.Content}");

        State.Number += 1;
    }
}