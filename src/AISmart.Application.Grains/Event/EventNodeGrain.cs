using AISmart.Domain.Grains.Event;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public class EventNodeGrain:  Grain<EventNodeState>,IEventNodeGrain
{
    private readonly IObjectMapper _objectMapper;
    
    public EventNodeGrain(IObjectMapper objectMapper
        )
    {
        _objectMapper = objectMapper;
    }
    public async Task CreateEventNode(EventNodeDto eventNodeDto)
    {
        State = _objectMapper.Map<EventNodeDto, EventNodeState>(eventNodeDto);
        await WriteStateAsync();
    }

    public async Task<EventNodeDto> GetEventNode()
    {
       return _objectMapper.Map<EventNodeState , EventNodeDto>(State);
    }
}