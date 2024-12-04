using AISmart.Domain.Grains.Event;

namespace AISmart.Application.Grains.Event;

using Orleans;
using System.Threading.Tasks;

public interface ITaskGrain : IGrainWithGuidKey
{
    Task<List<CreatedEvent>> CreateTask(Guid templateId, string param);
    
    Task<List<CreatedEvent>> CompleteEvent(Guid eventId,bool isSuccess, string failReason = null, string result = null);
    Task<TaskDto> GetTask();
}