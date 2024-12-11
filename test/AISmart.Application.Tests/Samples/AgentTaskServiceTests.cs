using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.ImplementationAgent.Events;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.AgentTask;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using AISmart.Sender;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Providers.Streams.Common;
using Orleans.Runtime;
using Orleans.Streams;
using Shouldly;
using Volo.Abp.EventBus.Local;
using Xunit;
using Xunit.Abstractions;
using IAgent = AISmart.Agents.IAgent;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase

{
    private readonly AgentTaskService _agentTaskService;
    private readonly IClusterClient _clusterClient;
    protected readonly IGrainFactory _grainFactory ;

    
    private readonly ILocalEventBus _localEventBus;
    private readonly ITestOutputHelper _output;

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
        _agentTaskService = GetRequiredService<AgentTaskService>();
        _clusterClient = GetRequiredService<IClusterClient>();
        _grainFactory = GetRequiredService<IGrainFactory>();
        
        _localEventBus = GetRequiredService<ILocalEventBus>();
        
        _output = output;
        
        // _senderTemplateId = Guid.NewGuid();
        // _senderAgent = _clusterClient.GetGrain<IAgent>(_senderTemplateId);
        
        // _receiverTemplateId = Guid.NewGuid();
        // _receiverAgent = _clusterClient.GetGrain<IAgent>(_receiverTemplateId);
        
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
    public async Task EventBus_Test()
    {
        BasicEvent basicEvent = new BasicEvent
        {
            AgentTopic = CommonConstants.CommonTopic,
            Downstreams = null,
            Content = "common message"
        };
        
        // await _senderAgent.PublishAsync(basicEvent);
        //
        // await Task.Delay(1000 * 5);
    }
    

    [Fact]
    public async Task Telegram_Test()
    {
        // TelegramEvent telegramEvent = new TelegramEvent
        // {
        //     Id = _tgTemplateId,
        //     AgentTopic = CommonConstants.TelegramTopic,
        //     Downstreams = null,
        //     Content = "比特币突破10万美元大关"
        // };
        // await _tgAgent.ChatAsync(telegramEvent);

    }
    
    [Fact]
    public async Task Multi_Agent_Test()
    {
        TelegramEvent telegramEvent = new TelegramEvent()
        {
            Id = _tgTemplateId,
            AgentTopic = CommonConstants.TelegramTopic,
            Downstreams = null,
            Content = "比特币突破10万美元大关"
        };
    }
    
    [Fact]
    public async Task AgentFlow_Test()
    {
        var xThreadCreatedEvent = new XThreadCreatedEvent()
        {
            Id = "mock_x_thread_id",
            Content = "BTC REACHED 100k WOOHOOOO!"
        };

        await _publishingAgent.PublishEventAsync(xThreadCreatedEvent);
        
        //TODO Expected from the unit tests
        await Task.Delay(1000 * 100);
    }
}