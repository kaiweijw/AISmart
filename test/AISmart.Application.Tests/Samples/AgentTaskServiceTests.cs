using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AISmart.AgentTask;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Orleans;
using Orleans.Providers.Streams.Common;
using Shouldly;
using Volo.Abp.EventBus.Local;
using Xunit;
using Xunit.Abstractions;

namespace AISmart.Samples;

public class AgentTaskServiceTests : AISmartApplicationTestBase

{
    private readonly AgentTaskService _agentTaskService;
    private readonly IClusterClient _clusterClient;
    
    private readonly ILocalEventBus _localEventBus;
    private readonly ITestOutputHelper _output;

    private readonly Guid _tgTemplateId;
    private readonly ITelegramAgent _tgAgent;
    
    private readonly Guid _marketTemplateId;
    private readonly IMarketLeaderAgent _marketLeaderAgent;
    
    private readonly Guid _marketOperatorTemplateId;
    
    private readonly IMarketOperatorAgent _marketOperatorAgent;
    private readonly IMarketLeaderStreamAgent _marketLeaderStreamAgent;
    
    private readonly Guid _senderTemplateId;
    private readonly IAgent _senderAgent;
    
    private readonly Guid _receiverTemplateId;
    private readonly IAgent _receiverAgent;

    public AgentTaskServiceTests(ITestOutputHelper output)
    {
        _agentTaskService = GetRequiredService<AgentTaskService>();
        _clusterClient = GetRequiredService<IClusterClient>();
        _localEventBus = GetRequiredService<ILocalEventBus>();
        
        _output = output;
        
        _senderTemplateId = Guid.NewGuid();
        _senderAgent = _clusterClient.GetGrain<IAgent>(_senderTemplateId);
        
        // _receiverTemplateId = Guid.NewGuid();
        // _receiverAgent = _clusterClient.GetGrain<IAgent>(_receiverTemplateId);
        
        
        
        _tgTemplateId = Guid.NewGuid();
        _tgAgent = _clusterClient.GetGrain<ITelegramAgent>(_tgTemplateId);
        
        _marketTemplateId = Guid.NewGuid();
        _marketLeaderAgent = _clusterClient.GetGrain<IMarketLeaderAgent>(_marketTemplateId);
        _marketLeaderAgent.CompelteStrategyAsync(null);
        
        // Guid _marketStreamTemplateId = Guid.NewGuid();
        // _marketLeaderStreamAgent = _clusterClient.GetGrain<IMarketLeaderStreamAgent>(_marketStreamTemplateId);
        // _marketLeaderStreamAgent.CompelteStrategyAsync(null);

        
        _marketOperatorTemplateId = Guid.NewGuid();
        _marketOperatorAgent = _clusterClient.GetGrain<IMarketOperatorAgent>(_marketOperatorTemplateId);
        
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
        
        await _senderAgent.PublishAsync(basicEvent);
        
        await Task.Delay(1000 * 5);
    }
    

    [Fact]
    public async Task Telegram_Test()
    {
        TelegramEvent telegramEvent = new TelegramEvent
        {
            Id = _tgTemplateId,
            AgentTopic = CommonConstants.TelegramTopic,
            Downstreams = null,
            Content = "比特币突破10万美元大关"
        };
        await _tgAgent.ChatAsync(telegramEvent);

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

        // await _tgAgent.ChatAsync(telegramEvent);
        // _localEventBus.PublishAsync(telegramEvent);
        await _senderAgent.PublishOrleansAsync(telegramEvent);

        ;
        // _senderAgent = _clusterClient.GetGrain<IAgent>(Guid.NewGuid());.
        
        // await _marketOperatorStreamAgent.ExecuteStrategyAsync(null);

        // await _marketLeaderStreamAgent.CompelteStrategyAsync(null);
        
        // await _marketLeaderAgent.CompelteStrategyAsync(null);
        
        // EventSequenceToken evetnset;
        // _localEventBus.Subscribe(telegramEvent, () =>
        //     {
        //         evetnset.Set
        //     }
        //     );
        // await evetnset.Task;
        await Task.Delay(1000 * 50);
    }
}