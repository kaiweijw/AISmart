using AISmart.Agents;
using AISmart.GAgent.Core;
using Microsoft.Extensions.Logging;
using Orleans.Storage;

namespace AISmart.Grains.Tests.TestGAgents;

[GenerateSerializer]
public class NaiveTestGAgentState
{
    [Id(0)]  public List<string> Content { get; set; }
}

public class NaiveTestGEvent : GEventBase
{
    [Id(0)] public Guid Id { get; set; }
}

[GAgent]
public class NaiveTestGAgent : GAgentBase<NaiveTestGAgentState, NaiveTestGEvent>
{
    public NaiveTestGAgent(ILogger logger, IGrainStorage grainStorage) : base(logger, grainStorage)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("This is a naive test GAgent");
    }
}