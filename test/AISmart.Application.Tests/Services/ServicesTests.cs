using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents.X;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Service;
using Microsoft.IdentityModel.Tokens;
using Orleans;
using Orleans.Runtime;
using Shouldly;
using Xunit;

namespace AISmart.Services;

public class ServicesTests : AISmartApplicationTestBase, IAsyncLifetime
{
    private readonly IClusterClient _clusterClient;
    protected readonly IGrainFactory _grainFactory;

    private IGAgentAppService _gAgentAppService;
    private IEventAppService _eventAppService;
    private IGroupAppService _groupAppService;

    public ServicesTests()
    {
        _clusterClient = GetRequiredService<IClusterClient>();
        _grainFactory = GetRequiredService<IGrainFactory>();
    }

    [Fact]
    public async Task PipelineTest()
    {
        var xGAgentGrainId = await _gAgentAppService.CreateAgentAsync(typeof(XGAgent).Namespace!);
        var marketLeaderGAgentGrainId = await _gAgentAppService.CreateAgentAsync(typeof(MarketLeaderGAgent).Namespace!);
        var developerGAgentGrainId = await _gAgentAppService.CreateAgentAsync(typeof(DeveloperGAgent).Namespace!);
        var investmentGAgentGrainId = await _gAgentAppService.CreateAgentAsync(typeof(InvestmentGAgent).Namespace!);
        
        var groupGAgentGrainId = await _groupAppService.CreateGroupAsync();

        await _groupAppService.SubscribeAgentToGroupAsync(xGAgentGrainId, typeof(XGAgent).Namespace!, groupGAgentGrainId);
        await _groupAppService.SubscribeAgentToGroupAsync(marketLeaderGAgentGrainId, typeof(MarketLeaderGAgent).Namespace!, groupGAgentGrainId);
        await _groupAppService.SubscribeAgentToGroupAsync(developerGAgentGrainId, typeof(DeveloperGAgent).Namespace!, groupGAgentGrainId);
        await _groupAppService.SubscribeAgentToGroupAsync(investmentGAgentGrainId, typeof(InvestmentGAgent).Namespace!, groupGAgentGrainId);
        
        await _eventAppService.PublishEventToGroupAsync(groupGAgentGrainId, "XThreadCreatedEvent", new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = "mock_content"
        });

        GrainTracker.XAgents.Count.ShouldBe(1);
        GrainTracker.XAgents.First().GetGrainId().GetGuidKey().ShouldBe(xGAgentGrainId);
    }

    [Fact]
    public async Task GetAllAvailableGAgentListTest()
    {
        var grainTypeList = await _gAgentAppService.GetAllAvailableGAgentListAsync();
    }

    public async Task InitializeAsync()
    {
        _gAgentAppService = GetRequiredService<IGAgentAppService>();
        _eventAppService = GetRequiredService<IEventAppService>();
        _groupAppService = GetRequiredService<IGroupAppService>();
    }

    public async Task DisposeAsync()
    {
    }
    
    private async Task<bool> CheckState(XAgentState state)
    {
        return !state.ThreadIds.IsNullOrEmpty();
    }
}