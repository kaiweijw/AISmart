using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Application.Grains.Event;
using AISmart.Sender;
using Orleans;
using Orleans.TestingHost.Utils;
using Shouldly;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class XAgentTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;
    protected readonly IGrainFactory _grainFactory ;

    private readonly IMarketLeaderStreamAgent _marketLeaderStreamAgent;
    private readonly IAgent<XAgentState> _xAgent;
    private readonly IAgent<MarketLeaderAgentState> _marketLeaderAgent;
    private readonly IAgent<DeveloperAgentState> _developerAgent;
    // private readonly IAgent<SocialEvent> _marketAgent;
    private readonly IPublishingAgent _publishingAgent;
    
    private readonly Guid XAgentGrainId = Guid.NewGuid();

    public XAgentTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        _grainFactory = GetRequiredService<IGrainFactory>();

        _xAgent = _grainFactory.GetGrain<IAgent<XAgentState>>(Guid.NewGuid());
        _xAgent.ActivateAsync();

        _marketLeaderAgent = _grainFactory.GetGrain<IAgent<MarketLeaderAgentState>>(Guid.NewGuid());
        _marketLeaderAgent.ActivateAsync();

        _developerAgent = _grainFactory.GetGrain<IAgent<DeveloperAgentState>>(Guid.NewGuid());
        _developerAgent.ActivateAsync();

        _publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
    }

    [Fact]
    public async Task XThreadCreatedEvent_Executed_Test()
    {
        const string content = "BTC REACHED 100k WOOHOOOO!";

        var xThreadCreatedEvent = new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = content
        };

        await _publishingAgent.PublishEventAsync(xThreadCreatedEvent);

        var developerAgent = GrainTracker.DeveloperAgents.First();
        var state = await developerAgent.GetStateAsync();

        await TestingUtils.WaitUntilAsync(_ => CheckState(state), TimeSpan.FromSeconds(20));
        state.Content.Count.ShouldBe(1);
    }

    private async Task<bool> CheckState(DeveloperAgentState state)
    {
        return !state.Content.IsNullOrEmpty();
    }
}