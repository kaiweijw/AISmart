using AISmart.Domain.Grains.Event;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;
using Volo.Abp.EventBus.Local;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public class Agent : Grain<BasicEvent>, IAgent,ITransientDependency
{
    private readonly IObjectMapper _objectMapper;
    
    // public ILocalEventBus EventBus { get; set; }
    
    private readonly ILocalEventBus _localEventBus;


    public Agent(IObjectMapper objectMapper,ILocalEventBus localEventBus) 
    {
        _objectMapper = objectMapper;
         _localEventBus = localEventBus;

    }

    protected Agent(IObjectMapper objectMapper)
    {
        _objectMapper = objectMapper;
    }


    public async Task PublishAsync(BasicEvent basicEvent)
    {
        Console.WriteLine($"Event Publish: {basicEvent.Content}");
        // await EventBus.PublishAsync(basicEvent);
        await _localEventBus.PublishAsync(basicEvent);
        await WriteStateAsync();
    }

    public async Task<BasicEvent> GetEventNodeAsync()
    {
        return _objectMapper.Map<BasicEvent, BasicEvent>(State);
    }

    public Task HandleEventAsync(BasicEvent eventData)
    {
        Console.WriteLine($"Event Received: {eventData.Content}");
        return Task.CompletedTask;
    }
}

