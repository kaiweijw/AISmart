using System;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Sender;
using Orleans;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;

    private readonly IAgent<XThreadCreatedEvent> _xAgent;
    private readonly IAgent<SocialEvent> _marketAgent;
    private readonly IInvestmentAgent _investmentAgent;
    private readonly IDeveloperAgent _developerAgent;
    private readonly IPublishingAgent _publishingAgent;

    public AgentTaskServiceTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        
        _xAgent = _clusterClient.GetGrain<IAgent<XThreadCreatedEvent>>(Guid.NewGuid());
        _xAgent.ActivateAsync();
        _marketAgent = _clusterClient.GetGrain<IAgent<SocialEvent>>(Guid.NewGuid());
        _marketAgent.ActivateAsync();
        
        _investmentAgent = _clusterClient.GetGrain<IInvestmentAgent>(Guid.NewGuid());
        _investmentAgent.ActivateAsync();
        
        _developerAgent = _clusterClient.GetGrain<IDeveloperAgent>(Guid.NewGuid());
        _developerAgent.ActivateAsync();
        
        _publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
    }

    [Fact]
    public async Task AgentFlow_Test()
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