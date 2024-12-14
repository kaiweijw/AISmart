

using System.Threading.Tasks;
using AISmart.Agents.X.Events;
using Google.Cloud.AIPlatform.V1;
using Orleans;
using Orleans.Providers;

namespace  AISmart.Mongo.Agent;

public interface ISenderAgent:IGrainWithGuidKey
{
    Task PublishEventAsync<T>(T @event);
}


[GenerateSerializer]
public class SenderAgentState
{
    [Id(0)]  public string Content { get; set; }

    public void Apply(XThreadCreatedEvent xThreadCreatedEvent)
    {
        
    }
}



[StorageProvider(ProviderName = "PubSubStore")]
// [LogConsistencyProvider(ProviderName = "LogStorage")]
[LogConsistencyProvider(ProviderName = "CustomStorage")]
public class SenderAgent : MongoStorage<SenderAgentState, XThreadCreatedEvent>, ISenderAgent
{
    public SenderAgent() : base()
    {
    }
    

    public Task PublishEventAsync<T>(T @event)
    {
        RaiseEvent((@event as XThreadCreatedEvent));
        base.ConfirmEvents();
        return Task.CompletedTask;
    }
}