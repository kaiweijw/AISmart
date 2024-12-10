using AISmart.Application.Grains.Agents.X.Events;
using Microsoft.Extensions.Logging;

namespace AISmart.Application.Grains.Agents.X;

public class XAgent : GAgent<XAgentState, XThreadCreatedEvent>
{
    public XAgent(ILogger<XAgent> logger,IClusterClient clusterClient) : base(logger,clusterClient)
    {
    }

    public override Task<string> GetDescriptionAsync()
    {
        return Task.FromResult("An agent to inform other agents when a X thread is published.");
    }

    protected override Task ExecuteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation("ExecuteAsync: X Thread {XContent}", eventData.Content);
        
        return Task.CompletedTask;
    }

    protected override Task CompleteAsync(XThreadCreatedEvent eventData)
    {
        Logger.LogInformation("CompleteAsync: X Thread {XContent}", eventData.Content);

        return Task.CompletedTask;
    }
}