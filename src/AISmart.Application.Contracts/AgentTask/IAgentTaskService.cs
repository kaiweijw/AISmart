using System;
using System.Threading.Tasks;
using AISmart.Domain.Grains.Event;

namespace AISmart.AgentTask;

public interface IAgentTaskService
{
      Task<Guid> CreateAgentTaskAsync(Guid TaskTemplateId,string param);

      Task<Guid> CompletedEventAsync(CreatedEvent createdEvent, bool isSuccess,
            string failReason, string result);
      Task<AgentTaskDto> GetAgentTaskDetailAsync(Guid guid);
}