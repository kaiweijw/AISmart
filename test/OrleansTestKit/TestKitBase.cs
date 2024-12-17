using AISmart.Agents;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.Sender;

namespace Orleans.TestKit;

/// <summary>A unit test base class that provides a default mock grain activation context.</summary>
public abstract class TestKitBase
{
    protected TestKitSilo Silo { get; } = new();

    protected async Task<PublishingGAgent> CreatePublishingGAgentAsync(params IGAgent[] gAgentsToPublish)
    {
        var publishingGAgent = await Silo.CreateGrainAsync<PublishingGAgent>(Guid.NewGuid());
        Silo.AddProbe<IPublishingGAgent>(_ => publishingGAgent);
        foreach (var gAgent in gAgentsToPublish)
        {
            await publishingGAgent.PublishTo(gAgent);
        }

        return publishingGAgent;
    }

    protected async Task<GroupGAgent> CreateGroupGAgentAsync(params IGAgent[] gAgentsToRegister)
    {
        var groupGAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        foreach (var gAgent in gAgentsToRegister)
        {
            await groupGAgent.Register(gAgent);
        }

        return groupGAgent;
    }
}