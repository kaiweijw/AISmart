using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agent.Event;
using AISmart.Agent.GEvents;
using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Application.Grains.Agents.Developer;
using Orleans;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.GAgent;

public class DeveloperGAgentTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ITestOutputHelper _output;
    public DeveloperGAgentTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        _output = output;
    }

    [Fact]
    public async Task ImplementationEventTest()
    {
        string chainId = "AELF";
        string senderName = "Test";
        var implementationEvent = new ImplementationEvent(){
            Content = "ImplementationEvent-Content"
        };
        var guid = Guid.NewGuid();
        await _clusterClient.GetGrain<IDeveloperAgent>(guid).ExecuteTransactionAsync(implementationEvent);
    }

}