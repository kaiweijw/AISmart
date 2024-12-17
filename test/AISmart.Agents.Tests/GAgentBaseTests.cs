using AISmart.Agents;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.Application.Grains.Agents.X;
using AISmart.GAgent.Autogen;
using AISmart.Grains.Tests.TestEvents;
using AISmart.Grains.Tests.TestGAgents;
using Orleans.TestKit;
using Shouldly;

namespace AISmart.Grains.Tests;

public class GAgentBaseTests : TestKitBase
{
    [Fact]
    public async Task EventHandlerTest()
    {
        var eventHandlerTestGAgent = await Silo.CreateGrainAsync<EventHandlerTestGAgent>(Guid.NewGuid());
        var subscribedEventList = await eventHandlerTestGAgent.GetAllSubscribedEventsAsync();
        subscribedEventList.ShouldNotBeNull();
        subscribedEventList.Count.ShouldBe(3);
        subscribedEventList.ShouldContain(typeof(NaiveTestEvent));
        subscribedEventList.ShouldContain(typeof(EventWrapperBase));
    }

    [Fact]
    public async Task PublishToEventHandlerTest()
    {
        var eventHandlerTestGAgent = await Silo.CreateGrainAsync<EventHandlerTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(eventHandlerTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);
        await publishingGAgent.PublishEventAsync(new NaiveTestEvent
        {
            Greeting = "Hello world"
        });
        var state = await eventHandlerTestGAgent.GetStateAsync();
        state.Content.Count.ShouldBe(3);
        state.Content.ShouldContain("Hello world");
    }
}