using AISmart.Agents;
using AISmart.Agents.X.Events;
using AISmart.Application.Grains.Agents.X;
using AISmart.Dapr;
using Orleans.TestKit;
using Shouldly;

namespace AISmart.Grains.Tests;

public class PipelineTest : TestKitBase
{
    [Fact]
    public async Task XAgentTest()
    {
        var stream =
            Silo.AddStreamProbe<EventWrapperBase>(CommonConstants.StreamGuid, CommonConstants.StreamNamespace);

        var xAgent = await Silo.CreateGrainAsync<XAgent>(Guid.NewGuid());
        await xAgent.ActivateAsync();

        var xThreadCreatedEvent = new XThreadCreatedEvent
        {
            Id = "mocked_id",
            Content = "BTC REACHED 100k WOOHOOOO!"
        };

        var eventWrapper = new EventWrapper<XThreadCreatedEvent>(xThreadCreatedEvent, xAgent.GetGrainId());

        await stream.OnNextAsync(eventWrapper);

        stream.Sends.ShouldBe(2u);

        var state = await xAgent.GetStateAsync();
        state.ThreadIds.Count.ShouldBe(1);
        state.ThreadIds.First().ShouldBe("mocked_id");
    }
}