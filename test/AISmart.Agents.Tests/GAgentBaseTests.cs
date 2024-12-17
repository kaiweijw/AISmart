using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Investment;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.Application.Grains.Agents.X;
using AISmart.GAgent.Autogen;
using Orleans.TestKit;
using Shouldly;

namespace AISmart.Grains.Tests;

public class GAgentBaseTests : TestKitBase
{
    [Fact]
    public async Task EventHandlerTest()
    {
        
    }
    
    [Fact]
    public async Task GetAllSubscribedEventsTest()
    {
        var xGAgent = await Silo.CreateGrainAsync<XGAgent>(Guid.NewGuid());
        var eventList = await xGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(1);

        var groupGAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        eventList = await groupGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(0);

        var publishingGAgent = await Silo.CreateGrainAsync<PublishingGAgent>(Guid.NewGuid());
        eventList = await publishingGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(0);

        var marketLeaderGAgent = await Silo.CreateGrainAsync<MarketLeaderGAgent>(Guid.NewGuid());
        eventList = await marketLeaderGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(1);

        var developerGAgent = await Silo.CreateGrainAsync<DeveloperGAgent>(Guid.NewGuid());
        eventList = await developerGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(1);

        var investmentGAgent = await Silo.CreateGrainAsync<InvestmentGAgent>(Guid.NewGuid());
        eventList = await investmentGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(1);
        
        var autoGenGAgent = await Silo.CreateGrainAsync<AutogenGAgent>(Guid.NewGuid());
        eventList = await autoGenGAgent.GetAllSubscribedEventsAsync();
        eventList.Count.ShouldBe(3);
    }
}