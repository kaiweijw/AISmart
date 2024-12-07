using AISmart.Domain.Grains.Event;

namespace AISmart.Application.Grains.Event;

using Orleans;
using System.Threading.Tasks;

public interface IAgentTaskGrain : IGrainWithGuidKey
{
    Task<List<CreatedEvent>> CreateAgentTaskAsync(Guid templateId, string param);
    
    Task<List<CreatedEvent>> CompletedEventAsync(CreatedEvent @event,  bool isSuccess,
        string failReason = null, string result = null);
    Task<AgentDto> GetAgentTaskDetailAsync();
}