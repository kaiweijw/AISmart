using AISmart.Agents.A.Events;
using AISmart.Application.Grains.Agents.A;
using AISmart.Application.Grains.Agents.B;
using AISmart.Application.Grains.Agents.Group;
using AISmart.Application.Grains.Agents.Publisher;
using AISmart.Application.Grains.Agents.C;
using AISmart.GGrains.Tests;
using Xunit.Abstractions;
using System.Diagnostics;

namespace AISmart.Grains.Tests.LoadTest;

public class PerformanceTest : GAgentTestKitBase
{
    private readonly ITestOutputHelper _testOutputHelper;

    public PerformanceTest(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task LoadTest()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        _testOutputHelper.WriteLine($"Test started at: {DateTime.Now}");

        var groupGAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var aGAgent = await Silo.CreateGrainAsync<AGAgent>(Guid.NewGuid());
        var bGAgent = await Silo.CreateGrainAsync<BGAgent>(Guid.NewGuid());
        var cGAgent = await Silo.CreateGrainAsync<CGAgent>(Guid.NewGuid());

        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(Guid.NewGuid());
        await groupGAgent.Register(aGAgent);
        await groupGAgent.Register(bGAgent);
        await groupGAgent.Register(cGAgent);
        await publishingAgent.PublishTo(groupGAgent);

        await publishingAgent.PublishEventAsync(new AThreadCreatedEvent
        {
            Id = $"mock_A_thread_id",
            Content = $"Testing iteration"
        });
        
        var aAgentState = await aGAgent.GetStateAsync();
        var bAgentState = await bGAgent.GetStateAsync();
        var cAgentState = await cGAgent.GetStateAsync();
        
        _testOutputHelper.WriteLine($"AGAgent State: {aAgentState.Number}");
        _testOutputHelper.WriteLine($"BGAgent State: {bAgentState.Number}");
        _testOutputHelper.WriteLine($"CGAgent State: {cAgentState.Number}");
        
        stopwatch.Stop();
        _testOutputHelper.WriteLine($"Test ended at: {DateTime.Now}");
        _testOutputHelper.WriteLine($"Total execution time: {stopwatch.ElapsedMilliseconds} ms");
    }
}