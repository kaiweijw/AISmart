using System;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Application.Grains.Event;
using AISmart.Sender;
using Orleans;
using Shouldly;
using Xunit;
using Xunit.Abstractions;
using IAgent = AISmart.Agents.IAgent;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase
{
    private readonly IClusterClient _clusterClient;
    private readonly ITestOutputHelper _output;
    protected readonly IGrainFactory _grainFactory ;

    private readonly Guid _tgTemplateId;
    // private readonly ITelegramAgent _tgAgent;
    //
    // private readonly Guid _marketTemplateId;
    // private readonly IMarketLeaderAgent _marketLeaderAgent;
    //
    // private readonly Guid _marketOperatorTemplateId;
    
    // private readonly IMarketOperatorAgent _marketOperatorAgent;
    // private readonly IMarketLeaderStreamAgent _marketLeaderStreamAgent;
    
    private readonly IAgent _xAgent;
    private readonly IAgent _marketAgent;
    private readonly IInvestmentAgent _investmentAgent;
    private readonly IDeveloperAgent _developerAgent;
    private readonly IAgent _developerAgent2;
    
    // internal IGrainRuntime Runtime { get; }
    //

    
    // private readonly Guid _senderTemplateId;
    // private readonly IAgent _senderAgent;
    //
    // private readonly Guid _receiverTemplateId;
    // private readonly IAgent _receiverAgent;
    private readonly IPublishingAgent _publishingAgent;

    public AgentTaskServiceTests(ITestOutputHelper output)
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        _grainFactory = GetRequiredService<IGrainFactory>();
        
        _xAgent = _clusterClient.GetGrain<IAgent>(Guid.NewGuid(),typeof(XAgent).Namespace);
        _xAgent.ActivateAsync();
        _marketAgent = _clusterClient.GetGrain<IAgent>(Guid.NewGuid(),typeof(MarketLeaderAgent).Namespace);
        _marketAgent.ActivateAsync();
        //
        // _marketAgent = _clusterClient.GetGrain<AISmart.Agents.IAgent>(Guid.NewGuid());
        // _marketAgent.ActivateAsync();
        
        // _grainFactory.CreateObjectReference<IAgent<ImplementationEvent>>(typeof(IAgent<ImplementationEvent> ))
        
        // _investmentAgent = _clusterClient.GetGrain<IInvestmentAgent>(Guid.NewGuid());
        // _investmentAgent.ActivateAsync();
        
        // _developerAgent = _clusterClient.GetGrain<IDeveloperAgent>(Guid.NewGuid());
        // _developerAgent.ActivateAsync();
        
        // _developerAgent = _grainFactory.GetGrain<IDeveloperAgent>(Guid.NewGuid(),"AISmart.Application.Grains.Agents.Developer");
        // _developerAgent.ActivateAsync();
        _developerAgent2  = _grainFactory.GetGrain<IAgent>(Guid.NewGuid(),typeof(DeveloperAgent).Namespace);
        _developerAgent2.ActivateAsync();
        // _tgTemplateId = Guid.NewGuid();
        // _tgAgent = _clusterClient.GetGrain<ITelegramAgent>(_tgTemplateId);
        
        // _marketTemplateId = Guid.NewGuid();
        // _marketLeaderAgent = _clusterClient.GetGrain<IMarketLeaderAgent>(_marketTemplateId);
        // _marketLeaderAgent.CompelteStrategyAsync(null);
        
        // Guid _marketStreamTemplateId = Guid.NewGuid();
        // _marketLeaderStreamAgent = _clusterClient.GetGrain<IMarketLeaderStreamAgent>(_marketStreamTemplateId);
        // _marketLeaderStreamAgent.CompelteStrategyAsync(null);

        
        // _marketOperatorTemplateId = Guid.NewGuid();
        // _marketOperatorAgent = _clusterClient.GetGrain<IMarketOperatorAgent>(_marketOperatorTemplateId);
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