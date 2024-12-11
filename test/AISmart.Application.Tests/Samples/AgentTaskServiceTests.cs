using System;
using System.Threading;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.AgentTask;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Event;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using AISmart.Sender;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Orleans;
using Shouldly;
using Volo.Abp.EventBus.Local;
using Xunit;
using Xunit.Abstractions;
using Moq;

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
    private readonly IAgent<XThreadCreatedEvent> _xAgent;
    private readonly IAgent<SocialEvent> _marketAgent;
    
    private readonly Guid _senderTemplateId;
    private readonly IAgent _senderAgent;
    
    private readonly Guid _receiverTemplateId;
    private readonly IAgent _receiverAgent;
    private readonly IPublishingAgent _publishingAgent;

    private readonly IServiceProvider _serviceProvider;
    private readonly TestAgent _testAgent;

    public AgentTaskServiceTests(ITestOutputHelper output)
    {
        _agentTaskService = GetRequiredService<AgentTaskService>();
        _clusterClient = GetRequiredService<IClusterClient>();
        _localEventBus = GetRequiredService<ILocalEventBus>();

        _output = output;

        _senderTemplateId = Guid.NewGuid();
        _senderAgent = _clusterClient.GetGrain<IAgent>(_senderTemplateId);

        _xAgent = _clusterClient.GetGrain<IAgent<XThreadCreatedEvent>>(Guid.NewGuid());
        _xAgent.ActivateAsync();
        _marketAgent = _clusterClient.GetGrain<IAgent<SocialEvent>>(Guid.NewGuid());
        _marketAgent.ActivateAsync();

        _tgTemplateId = Guid.NewGuid();
        _tgAgent = _clusterClient.GetGrain<ITelegramAgent>(_tgTemplateId);

        _marketTemplateId = Guid.NewGuid();
        _marketLeaderAgent = _clusterClient.GetGrain<IMarketLeaderAgent>(_marketTemplateId);
        _marketLeaderAgent.CompelteStrategyAsync(null);

        _marketOperatorTemplateId = Guid.NewGuid();
        _marketOperatorAgent = _clusterClient.GetGrain<IMarketOperatorAgent>(_marketOperatorTemplateId);

        var testAgent = _clusterClient.GetGrain<IAgent<TestEvent>>(Guid.NewGuid());
        testAgent.ActivateAsync();

        _publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());

        var loggerMock = new Mock<ILogger<TestAgent>>();
        var clusterClientMock = new Mock<IClusterClient>();
        _testAgent = new TestAgent(loggerMock.Object, clusterClientMock.Object);
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IAgent<TestEvent>>(_testAgent);
        _serviceProvider = serviceCollection.BuildServiceProvider();
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
    }

    [Fact]
    public async Task AgentFlow_Test()
    {
        var resetEvent = new ManualResetEventSlim(false);

        // Arrange
        var testEvent = new TestEvent
        {
            Message = "Test message."
        };

        _testAgent.ExecuteAsyncCalled += () => resetEvent.Set();

        // Act
        await _publishingAgent.PublishEventAsync(testEvent);

        // Wait for the ExecuteAsync method to be called
        resetEvent.Wait(10000);

        // Assert
        TestAgent.CalledExecuteAsync.ShouldBeTrue();
    }
}

[GenerateSerializer]
public class TestEvent : GEvent
{
    public string Message { get; set; }
}

[GenerateSerializer]
public class TestAgentState;

public class TestAgent : GAgent<TestAgentState, TestEvent>
{
    public static bool CalledExecuteAsync;
    public static bool CalledCompleteAsync;

    public event Action ExecuteAsyncCalled;

    public TestAgent(ILogger<TestAgent> logger, IClusterClient clusterClient) : base(logger, clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("Test agent for unit testing.");
    }

    protected override Task ExecuteAsync(TestEvent eventData)
    {
        CalledExecuteAsync = true;
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(TestEvent eventData)
    {
        CalledCompleteAsync = true;
        return Task.CompletedTask;
    }
}