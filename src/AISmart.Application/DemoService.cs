using System;
using System.Linq;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Developer;
using AISmart.Agents.Group;
using AISmart.Agents.Investment;
using AISmart.Agents.MarketLeader;
using AISmart.Agents.X;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;
using AISmart.Sender;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Application;

public interface IDemoAppService
{
    Task<string> PipelineDemoAsync(string content);
}

public class DemoAppService : ApplicationService, IDemoAppService
{
    private readonly IClusterClient _clusterClient;

    public DemoAppService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task<string> PipelineDemoAsync(string content)
    {
        var xAgent = _clusterClient.GetGrain<IStateAgent<XAgentState>>(Guid.NewGuid(), typeof(XGAgent).Namespace);
        var marketLeaderAgent =
            _clusterClient.GetGrain<IStateAgent<MarketLeaderAgentState>>(Guid.NewGuid(),
                typeof(MarketLeaderGAgent).Namespace);
        var developerAgent =
            _clusterClient.GetGrain<IStateAgent<DeveloperAgentState>>(Guid.NewGuid(),
                typeof(DeveloperGAgent).Namespace);
        var investmentAgent =
            _clusterClient.GetGrain<IStateAgent<InvestmentAgentState>>(Guid.NewGuid(),
                typeof(DeveloperGAgent).Namespace);
        var publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
        var groupAgent = _clusterClient.GetGrain<IStateAgent<GroupAgentState>>(Guid.NewGuid());

        await groupAgent.Register(xAgent);
        await groupAgent.Register(marketLeaderAgent);
        await groupAgent.Register(developerAgent);
        await groupAgent.Register(investmentAgent);

        await publishingAgent.PublishTo(groupAgent);

        await publishingAgent.PublishEventAsync(new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = content
        });

        var investmentAgentState = await investmentAgent.GetStateAsync();
        return investmentAgentState.Content.First();
    }
}