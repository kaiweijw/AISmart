using System.ComponentModel;
using System.Runtime.CompilerServices;
using AISmart.Agents;
using AISmart.Application.Grains;
using Microsoft.Extensions.Logging;
using Orleans.Providers;

namespace AISmart.Grains.Tests.AutoGenTest;

[Description("I can draw a polygon")]
[StorageProvider(ProviderName = "PubSubStore")]
[LogConsistencyProvider(ProviderName = "LogStorage")]
public class DrawOperationGAgent : GAgentBase<DrawOperationState, DrawOperateEvent>
{
    public DrawOperationGAgent(ILogger logger) : base(logger)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        throw new NotImplementedException();
    }

    [EventHandler]
    public async Task ExecuteAsync(DrawTriangleEvent drawTriangleEvent)
    {
        await PublishAsync(new DrawTriangleResultEvent()
        {
            DrawTriangleContent = $"I Have draw a {drawTriangleEvent.PolygonSides} Triangle"
        });
    }
}

[Serializable]
public class DrawOperationState
{
}

[Serializable]
public class DrawOperateEvent : GEventBase
{
}

[Serializable]
[Description("Draw Triangle")]
public class DrawTriangleEvent : EventBase
{
    [Description("Sides of a polygon")]
    public int PolygonSides { get; set; }
}

[Serializable]
public class DrawTriangleResultEvent : EventBase
{
    public string DrawTriangleContent { get; set; }
}