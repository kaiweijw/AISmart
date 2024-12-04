using AISmart.Domain.Grains.Event;

namespace AISmart.Application.Grains.Event;

public interface IEventFlowTemplateGrain : IGrainWithGuidKey
{
    public Task CreateEventNodeAsync(EventFlowTemplateDto eventFlowTemplateDto);
    
    
    public Task<EventFlowTemplateDto> GetEventNodeAsync();

}