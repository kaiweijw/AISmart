using System;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Sender;
using Orleans;
using Shouldly;
using Volo.Abp.EventBus.Local;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class XAgentTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;
    private readonly IMarketLeaderStreamAgent _marketLeaderStreamAgent;
    private readonly IAgent<XThreadCreatedEvent> _xAgent;
    private readonly IAgent<SocialEvent> _marketAgent;
    private readonly IPublishingAgent _publishingAgent;
    
    public XAgentTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        
        _xAgent = _clusterClient.GetGrain<IAgent<XThreadCreatedEvent>>(Guid.NewGuid());
        _xAgent.ActivateAsync();
        
        _publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
    }
    
    [Fact]
    public async Task XThreadCreatedEvent_Executed_Test()
    {
        const string content = "BTC REACHED 100k WOOHOOOO!";
        const string expectedLog =
            $"AISmart.Application.Grains.Agents.X.XAgent ExecuteAsync: X Thread {content}";

        var xThreadCreatedEvent = new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = content
        };

        await _publishingAgent.PublishEventAsync(xThreadCreatedEvent);

        await ClusterFixture.WaitLogAsync(expectedLog);
        ClusterFixture.LoggerProvider.Logs.Any(log => log.Contains(content)).ShouldBeTrue();
    }
}