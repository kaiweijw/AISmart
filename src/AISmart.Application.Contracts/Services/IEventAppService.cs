using System;
using System.Threading.Tasks;

namespace AISmart.Services;

public interface IEventAppService
{
    Task PublishEventToGroupAsync(Guid groupId, string eventType, object eventData);
}
