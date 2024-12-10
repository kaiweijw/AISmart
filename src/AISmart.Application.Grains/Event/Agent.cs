using System.Diagnostics;
using AISmart.Dapr;
using AISmart.Domain.Grains.Event;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Runtime;
using Orleans.Streams;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;
using YamlDotNet.Core;

namespace AISmart.Application.Grains.Event;

public class Agent : Grain<BasicEvent>, IAgent,ITransientDependency
{
    
    // public ILocalEventBus EventBus { get; set; }
    
    private readonly ILocalEventBus _localEventBus;
    
    private IAsyncStream<BasicEvent>? _stream;



    public Agent(ILocalEventBus localEventBus) 
    {
         _localEventBus = localEventBus;
    }


    public async Task PublishAsync(BasicEvent basicEvent)
    {
        Console.WriteLine($"Event Publish: {basicEvent.Content}");
        // await EventBus.PublishAsync(basicEvent);
        await _localEventBus.PublishAsync(basicEvent);
        await WriteStateAsync();
    }



    public Task HandleEventAsync(BasicEvent eventData)
    {
        Console.WriteLine($"Agent Event Received: {eventData.Content}");
        return Task.CompletedTask;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamId = StreamId.Create(CommonConstants.StreamNamespace, CommonConstants.StreamGuid);
        _stream = this.GetStreamProvider(CommonConstants.StreamProvider)
            .GetStream<BasicEvent>(streamId);
    }
    
    public async Task PublishOrleansAsync(BasicEvent basicEvent)
    {
        Debug.Assert(_stream != null, nameof(_stream) + " != null");
        Console.WriteLine($"Orleans Agent Publish  Event : {basicEvent.Content}");
        await _stream.OnNextAsync(basicEvent);
    }
}

