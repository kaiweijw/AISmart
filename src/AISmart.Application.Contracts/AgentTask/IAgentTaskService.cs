using System;
using System.Threading.Tasks;
using AISmart.Domain.Grains.Event;

namespace AISmart.AgentTask;

public interface IAgentTaskService
{
      Task<Guid> CreateAgentTaskAsync(Guid TaskTemplateId,string param);

      Task<Guid> CompletedEventAsync(Guid taskId, Guid eventId, bool isSuccess,
            string failReason, string result);
      Task<TaskDto> GetAgentTaskDetailAsync(Guid guid);
}