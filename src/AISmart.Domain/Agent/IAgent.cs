using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Agent.Ability;
using AISmart.Agent.Coordinator;
using AISmart.AgentCommunicationComponent;
using AISmart.Events;

namespace AISmart.Agent;

public interface IAgent : IAgentEventCommunicationComponent, IAgentSolutionComponent,
    IAgentSolutionCommunicationComponent, ICoordinator
{
    Guid Id { get; }

    string AgentName { get; }

    Guid ParentAgentId { get; }

    Task<Tuple<bool, List<EventHandleResult>>> HandleEvent(Event @event, AbilityParam abilityParam);
}