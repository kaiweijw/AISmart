using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Events;

namespace AISmart.Agent.Coordinator;

public interface ICoordinator
{
    Task<CoordinatorResult> Coordinator(Event eventDescription, List<IAgent> agents);
}