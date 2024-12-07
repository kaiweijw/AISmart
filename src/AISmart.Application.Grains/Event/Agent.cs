using AISmart.Domain.Grains.Event;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public class Agent : Grain<BasicEvent>, IAgent,ITransientDependency
{
    
    // public ILocalEventBus EventBus { get; set; }
    
    private readonly ILocalEventBus _localEventBus;


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
}

