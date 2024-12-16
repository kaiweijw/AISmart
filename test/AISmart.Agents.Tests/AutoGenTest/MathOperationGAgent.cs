using System.ComponentModel;
using System.Runtime.CompilerServices;
using AISmart.Agents;
using AISmart.Application.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Grains.Tests.AutoGenTest;

[Description("i can add two integer")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class MathOperationGAgent: GAgentBase<MathOperationState, MathOperationEvent>
{
    public MathOperationGAgent(ILogger logger) : base(logger)
    {
    }
    
    [EventHandler]
    public async Task ExecuteAsync(AddNumberEvent numberEvent)
    {
        await PublishAsync(new AddNumberResultEvent()
        {
            Total = numberEvent.B + numberEvent.A + 1
        });
    }

    public override Task<string> GetDescriptionAsync()
    {
        throw new NotImplementedException();
    }
}

[GenerateSerializer]
public class MathOperationState
{
}

public class MathOperationEvent : GEventBase
{
}

[Serializable]
[Description("add Two number")]
[assembly: InternalsVisibleTo("AnotherAssembly")]
public class AddNumberEvent : EventBase
{
    [Description("First Number")]
    public int A { get; set; }
    [Description("Second Number")]
    public int B { get; set; }
}

[Serializable]
[Description("Sub Two number")]
[assembly: InternalsVisibleTo("AnotherAssembly")]
public class SubNumberEvent : EventBase
{
    [Description("First Number")]
    public int A { get; set; }
    [Description("Second Number")]
    public int B { get; set; }
}

[Serializable]
public class AddNumberResultEvent : EventBase
{
    public int Total { get; set; }
}