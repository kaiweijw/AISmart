using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Sender;
using AISmart.Application.Grains.Agents.X;
using Orleans.TestKit;
using Shouldly;

namespace AISmart.Grains.Tests;

public class PipelineTest : TestKitBase
{
    [Fact]
    public async Task GroupTest()
    {
        var groupAgent = await Silo.CreateGrainAsync<GroupAgent>(Guid.NewGuid());
        var publishingAgent = await Silo.CreateGrainAsync<PublishingAgent>(Guid.NewGuid());
        var xAgent = await Silo.CreateGrainAsync<XAgent>(Guid.NewGuid());

        await groupAgent.Register(xAgent);
        await publishingAgent.PublishTo(groupAgent);

        var xThreadCreatedEvent = new XThreadCreatedEvent
        {
            Id = "mock_x_thread_id",
            Content = "BTC REACHED 100k WOOHOOOO!"
        };

        await publishingAgent.PublishEventAsync(xThreadCreatedEvent);

        var xAgentState = await xAgent.GetStateAsync();
        xAgentState.ThreadIds.Count.ShouldBe(1);
    }
}