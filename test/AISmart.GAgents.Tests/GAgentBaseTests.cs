using AISmart.Agents;
using AISmart.GAgents.Tests;
using AISmart.GAgents.Tests.TestEvents;
using AISmart.GAgents.Tests.TestGAgents;
using Shouldly;

namespace AISmart.GAgents.Tests;

[Trait("Category", "BVT")]
public class GAgentBaseTests : GAgentTestKitBase
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

    [Fact]
    public async Task UnregisterTest()
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
        await groupGAgent.Unregister(eventHandlerTestGAgent);
        await publishingGAgent.PublishEventAsync(new NaiveTestEvent
        {
            Greeting = "Hello world"
        });
        state.Content.Count.ShouldBe(3);
    }

    [Fact]
    public async Task EventWithResponseTest()
    {
        var eventHandlerTestGAgent = await Silo.CreateGrainAsync<EventHandlerTestGAgent>(Guid.NewGuid());
        var eventHandlerWithResponseTestGAgent =
            await Silo.CreateGrainAsync<EventHandlerWithResponseTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(eventHandlerTestGAgent, eventHandlerWithResponseTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);
        await publishingGAgent.PublishEventAsync(new ResponseTestEvent
        {
            Greeting = "Hello, this is AISmart."
        });
        // Events will forward to eventHandlerTestGAgent
        var state = await eventHandlerTestGAgent.GetStateAsync();
        state.Content.Count.ShouldBe(4);
        state.Content.ShouldContain("Hello, this is AISmart.");
        state.Content.ShouldContain(content => content.Contains(nameof(ResponseTestEvent)));
        state.Content.ShouldContain(content => content.Contains(nameof(NaiveTestEvent)));
    }

    [Fact]
    public async Task BadEventHandlerTest()
    {
        var badEventHandlerTestGAgent = await Silo.CreateGrainAsync<BadEventHandlerTestGAgent>(Guid.NewGuid());
        var subscribedEventList = await badEventHandlerTestGAgent.GetAllSubscribedEventsAsync();
        subscribedEventList.ShouldNotBeNull();
        subscribedEventList.Count.ShouldBe(0);
    }

    [Fact]
    public async Task ResponseReturnTypeNotInheritedFromEventBaseTest()
    {
        var badEventHandlerTestGAgent = await Silo.CreateGrainAsync<FatalEventHandlerTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(badEventHandlerTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        var exception = Should.Throw<InvalidOperationException>(async () =>
        {
            await publishingGAgent.PublishEventAsync(new ResponseTestEvent
            {
                Greeting = "Expecting an exception."
            });
        });
        exception.Message.ShouldContain("return type needs to be inherited from EventBase.");
    }
    
    [Fact]
    public async Task ResponseEventNoResponseTypeTest()
    {
        var badEventHandlerTestGAgent = await Silo.CreateGrainAsync<FatalEventHandlerTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(badEventHandlerTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        var exception = Should.Throw<InvalidOperationException>(async () =>
        {
            await publishingGAgent.PublishEventAsync(new AnotherResponseTestEvent
            {
                Greeting = "Expecting an exception."
            });
        });
        exception.Message.ShouldContain("needs to have a return value.");
    }

    [Fact]
    public async Task RequestSubscribedEventListTest()
    {
        var eventHandlerTestGAgent = await Silo.CreateGrainAsync<EventHandlerTestGAgent>(Guid.NewGuid());
        var eventHandlerWithResponseTestGAgent =
            await Silo.CreateGrainAsync<EventHandlerWithResponseTestGAgent>(Guid.NewGuid());
        var subscribeTestGAgent = await Silo.CreateGrainAsync<SubscribeTestGAgent>(Guid.NewGuid());
        var groupGAgent = await CreateGroupGAgentAsync(eventHandlerTestGAgent, eventHandlerWithResponseTestGAgent,
            subscribeTestGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        AddProbesByGrainId(eventHandlerTestGAgent, eventHandlerWithResponseTestGAgent, subscribeTestGAgent, groupGAgent,
            publishingGAgent);

        await publishingGAgent.PublishEventAsync(new RequestAllSubscriptionsEvent());
        var state = await subscribeTestGAgent.GetStateAsync();
        state.SubscriptionInfo.Count.ShouldBe(3);
        state.SubscriptionInfo[typeof(EventHandlerTestGAgent)].Count.ShouldBe(3);
        state.SubscriptionInfo[typeof(EventHandlerWithResponseTestGAgent)].Count.ShouldBe(1);
        state.SubscriptionInfo[typeof(SubscribeTestGAgent)].Count.ShouldBe(1);
    }

    [Fact]
    public async Task LogViewAdaptorTest()
    {
        var guid = Guid.NewGuid();
        var logViewGAgent = await Silo.CreateGrainAsync<LogViewAdaptorTestGAgent>(guid);
        var groupGAgent = await CreateGroupGAgentAsync(logViewGAgent);
        var publishingGAgent = await CreatePublishingGAgentAsync(groupGAgent);

        await publishingGAgent.PublishEventAsync(new NaiveTestEvent
        {
            Greeting = "First event"
        });

        await TestHelper.WaitUntilAsync(_ => CheckCount(1));
        Silo.TestLogConsistentStorage.Storage.Count.ShouldBe(1);
        Silo.TestLogConsistentStorage.Storage.First().Value.Count.ShouldBe(1);
        (await GetLatestVersionAsync()).ShouldBe(0);

        await Silo.DeactivateAsync(logViewGAgent);
        logViewGAgent = await Silo.CreateGrainAsync<LogViewAdaptorTestGAgent>(guid);

        await publishingGAgent.PublishEventAsync(new NaiveTestEvent
        {
            Greeting = "Second event"
        });

        await TestHelper.WaitUntilAsync(_ => CheckCount(2));

        Silo.TestLogConsistentStorage.Storage.Count.ShouldBe(1);
        Silo.TestLogConsistentStorage.Storage.Last().Value.Count.ShouldBe(2);

        var logViewGAgentState = await logViewGAgent.GetStateAsync();
        await TestHelper.WaitUntilAsync(_ => CheckCount(logViewGAgentState, 2));
        logViewGAgentState.Content.Count.ShouldBe(2);

        (await GetLatestVersionAsync()).ShouldBe(1);

        await publishingGAgent.PublishEventAsync(new NaiveTestEvent
        {
            Greeting = "Third event"
        });

        await TestHelper.WaitUntilAsync(_ => CheckCount(3));

        (await GetLatestVersionAsync()).ShouldBe(2);
    }

    private async Task<bool> CheckCount(int expectedCount)
    {
        return Silo.TestLogConsistentStorage.Storage.Count == 1
               && Silo.TestLogConsistentStorage.Storage.Last().Value.Count == expectedCount;
    }

    private async Task<bool> CheckCount(LogViewAdaptorTestGState state, int expectedCount)
    {
        return state.Content.Count == expectedCount;
    }

    private async Task<int> GetLatestVersionAsync()
    {
        return await Silo.TestLogConsistentStorage.GetLastVersionAsync(string.Empty,
            GrainId.Create(string.Empty, string.Empty));
    }
}