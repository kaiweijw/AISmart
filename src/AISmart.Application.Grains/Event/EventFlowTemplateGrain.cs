using AISmart.Domain.Grains.Event;
using Volo.Abp.ObjectMapping;

namespace AISmart.Application.Grains.Event;

public class EventFlowTemplateGrain:  Grain<EventFlowTemplateState>,IEventFlowTemplateGrain
{
    private readonly IObjectMapper _objectMapper;
    
    public EventFlowTemplateGrain(IObjectMapper objectMapper
        )
    {
        _objectMapper = objectMapper;
    }
    public async Task CreateEventNodeAsync(EventFlowTemplateDto eventFlowTemplateDto)
    {
        State = _objectMapper.Map<EventFlowTemplateDto, EventFlowTemplateState>(eventFlowTemplateDto);
        await WriteStateAsync();
    }

    public async Task<EventFlowTemplateDto> GetEventNodeAsync()
    {
       return _objectMapper.Map<EventFlowTemplateState , EventFlowTemplateDto>(State);
    }
}