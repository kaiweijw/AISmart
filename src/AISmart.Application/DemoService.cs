using System;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.MarketLeader.Events;
using AISmart.Agents.X.Events;
using AISmart.Sender;
using Orleans;
using Volo.Abp.Application.Services;
using Volo.Abp.DependencyInjection;

namespace AISmart.Application;

public interface IDemoAppService
{
    Task<string> PipelineDemoAsync(string content);
}

public class DemoAppService : ApplicationService, IDemoAppService, ISingletonDependency
{
    private readonly IClusterClient _clusterClient;

    public DemoAppService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }

    public async Task<string> PipelineDemoAsync(string content)
    {
        var xAgent = _clusterClient.GetGrain<IAgent<XThreadCreatedEvent>>(Guid.NewGuid());
        await xAgent.ActivateAsync();

        var marketLeaderAgent = _clusterClient.GetGrain<IAgent<SocialEvent>>(Guid.NewGuid());
        await marketLeaderAgent.ActivateAsync();

        var publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
        await publishingAgent.PublishEventAsync(new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = content
        });

        return "Demo test.";
    }
}