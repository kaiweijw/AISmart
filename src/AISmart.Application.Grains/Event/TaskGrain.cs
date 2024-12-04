using AISmart.Domain.Grains.Event;

namespace AISmart.Application.Grains.Event;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orleans.EventSourcing;

public class TaskGrain : JournaledGrain<TaskState, TaskEvent>, ITaskGrain
{
    public async Task<List<CreatedEvent>> CreateTask(Guid templateId, string param)
    {
        var eventNodeDto = await GrainFactory.GetGrain<IEventNodeGrain>(templateId).GetEventNode();
        var createTaskEvent = new CreatedEvent
        {
            TaskId = this.GetPrimaryKey(),
            TemplateId = eventNodeDto.Id,
            Name = eventNodeDto.AgentTopic,
            IsCompleted = false,
            Param = param
        };
        base.RaiseEvent(createTaskEvent);
        await ConfirmEvents();
        List<CreatedEvent> taskEvents = new List<CreatedEvent>();
        taskEvents.Add(createTaskEvent);
        return taskEvents;
    }

    public async Task<List<CreatedEvent>> CompleteEvent(Guid eventId,bool isSuccess, string failReason = null, string result = null)
    {
        List<TaskEvent> events = new List<TaskEvent>();
        var completedTaskEvent = new CompletedTaskEvent
        {
            CreatedEventId = eventId,
            TaskId = this.GetPrimaryKey(),
            IsSuccess = isSuccess,
            FailReason = failReason,
            Result = result
        };
        events.Add(completedTaskEvent);
        var taskEvents =  await GetSubTaskEventsAsync(State, completedTaskEvent);
        if (!taskEvents.IsNullOrEmpty())
        {
            events.AddRange(taskEvents);
        }

        base.RaiseEvents(events);
        await ConfirmEvents();
        return  taskEvents;
    }

   
    protected override void TransitionState(
        TaskState state, TaskEvent @event)
    {
        switch (@event)
        {
            case CreatedEvent createEvent:
                State.Id = createEvent.TaskId;
                State.ProcessingEvents ??= new List<Guid>();
                State.ProcessingEvents.Add(createEvent.Id);
                break;

            case CompletedTaskEvent completeEvent:
                if (completeEvent.IsSuccess)
                {
                    State.EventResultDictionary ??= new Dictionary<Guid, EventResult>();
                    State.EventResultDictionary.Add(completeEvent.CreatedEventId,
                        new EventResult()
                        {
                           Result = completeEvent.Result,
                          // AgentTopic = completeEvent.a
                        });
                }
                else
                {
                    State.State = $"{completeEvent.Name} Failed: {completeEvent.FailReason}";
                    State.FailedEvents ??= new List<Guid>();
                    State.FailedEvents.Add(completeEvent.CreatedEventId);
                }
                State.ProcessingEvents.Remove(completeEvent.CreatedEventId);
                break;
        }
    }

    private async Task<List<CreatedEvent>> GetSubTaskEventsAsync(TaskState state, CompletedTaskEvent completeEvent)
    {
        List<CreatedEvent> taskEvents = new List<CreatedEvent>();
        var eventNodeDto = await GrainFactory.GetGrain<IEventNodeGrain>(completeEvent.TemplateId).GetEventNode();
        if (!eventNodeDto.Downstreams.IsNullOrEmpty())
        {
            foreach (var guid in eventNodeDto.Downstreams)
            {
               var taskEvent =  new CreatedEvent
                {
                    Id = Guid.NewGuid(),
                    TemplateId = guid,
                    TaskId = this.GetPrimaryKey(),
                    Name = completeEvent.Name,
                    IsCompleted = false,
                    Param = completeEvent.Result
                };
                taskEvents.Add(taskEvent);
            }
        }

        return taskEvents;
    }

    public Task<string> GetState()
    {
        return Task.FromResult(State.State);
    }
}