using AISmart.Domain.Grains.Event;

namespace AISmart.Application.Grains.Event;

public interface IEventNodeGrain : IGrainWithGuidKey
{
    public Task CreateEventNode(EventNodeDto eventNodeDto);
    
    
    public Task<EventNodeDto> GetEventNode();

}