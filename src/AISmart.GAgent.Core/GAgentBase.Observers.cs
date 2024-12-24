using System.Reflection;
using AISmart.Agents;
using Microsoft.Extensions.Logging;

namespace AISmart.GAgent.Core;

public abstract partial class GAgentBase<TState, TEvent>
{
    private Task UpdateObserverList()
    {
        var eventHandlerMethods = GetEventHandlerMethods();

        foreach (var eventHandlerMethod in eventHandlerMethods)
        {
            var observer = new EventWrapperBaseAsyncObserver(async item =>
            {
                var grainId = (Guid)item.GetType().GetProperty(nameof(EventWrapper<EventBase>.GrainId))?.GetValue(item)!;
                if (grainId == this.GetPrimaryKey())
                {
                    // Skip the event if it is sent by itself.
                    return;
                }

                var eventId = (Guid)item.GetType().GetProperty(nameof(EventWrapper<EventBase>.EventId))?.GetValue(item)!;
                var eventType = item.GetType().GetProperty(nameof(EventWrapper<EventBase>.Event))?.GetValue(item);
                var parameter = eventHandlerMethod.GetParameters()[0];

                var contextStorageGrainIdValue = item.GetType()
                    .GetProperty(nameof(EventWrapper<EventBase>.ContextStorageGrainId))?
                    .GetValue(item);
                GrainId? contextStorageGrainId = null;
                if (contextStorageGrainIdValue != null)
                {
                    contextStorageGrainId = (GrainId)contextStorageGrainIdValue;
                    var contextStorageGrain = GrainFactory.GetGrain<IContextStorageGrain>(contextStorageGrainId.Value.GetGuidKey());
                    if (contextStorageGrain == null)
                    {
                        throw new InvalidOperationException("Error while getting instance of IContextStorageGrain.");
                    }
                    var context = await contextStorageGrain.GetContext();
                    (eventType! as EventBase)!.SetContext(context);
                }

                if (parameter.ParameterType == eventType!.GetType())
                {
                    await HandleMethodInvocationAsync(eventHandlerMethod, parameter, eventType, eventId,
                        contextStorageGrainId);
                }

                if (parameter.ParameterType == typeof(EventWrapperBase))
                {
                    try
                    {
                        var invokeParameter =
                            new EventWrapper<EventBase>((EventBase)eventType, eventId, this.GetPrimaryKey(),
                                contextStorageGrainId);
                        var result = eventHandlerMethod.Invoke(this, [invokeParameter]);
                        await (Task)result!;
                    }
                    catch (Exception ex)
                    {
                        // TODO: Make this better.
                        Logger.LogError(ex, "Error invoking method {MethodName} with event type {EventType}",
                            eventHandlerMethod.Name, eventType.GetType().Name);
                    }
                }

                ClearContext();
            });

            Observers.Add(observer, new Dictionary<StreamId, Guid>());
        }

        return Task.CompletedTask;
    }

    private IEnumerable<MethodInfo> GetEventHandlerMethods()
    {
        return GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
            .Where(IsEventHandlerMethod);
    }

    private bool IsEventHandlerMethod(MethodInfo methodInfo)
    {
        return methodInfo.GetParameters().Length == 1 && (
            // Either the method has the EventHandlerAttribute
            // Or is named HandleEventAsync
            //     and the parameter is not EventWrapperBase 
            //     and the parameter is inherited from EventBase
            ((methodInfo.GetCustomAttribute<EventHandlerAttribute>() != null ||
              methodInfo.Name == AISmartGAgentConstants.EventHandlerDefaultMethodName) &&
             methodInfo.GetParameters()[0].ParameterType != typeof(EventWrapperBase) &&
             typeof(EventBase).IsAssignableFrom(methodInfo.GetParameters()[0].ParameterType))
            // Or the method has the AllEventHandlerAttribute and the parameter is EventWrapperBase
            || (methodInfo.GetCustomAttribute<AllEventHandlerAttribute>() != null &&
                methodInfo.GetParameters()[0].ParameterType == typeof(EventWrapperBase)));
    }

    private async Task HandleMethodInvocationAsync(MethodInfo method, ParameterInfo parameter, object eventType,
        Guid eventId, GrainId? contextStorageGrainId)
    {
        if (IsEventWithResponse(parameter))
        {
            await HandleEventWithResponseAsync(method, eventType, eventId, contextStorageGrainId);
        }
        else if (method.ReturnType == typeof(Task))
        {
            try
            {
                SetContextStorageGrainId(contextStorageGrainId);
                var result = method.Invoke(this, [eventType]);
                await (Task)result!;
            }
            catch (Exception ex)
            {
                // TODO: Make this better.
                Logger.LogError(ex, "Error invoking method {MethodName} with event type {EventType}", method.Name,
                    eventType.GetType().Name);
            }
        }
    }

    private bool IsEventWithResponse(ParameterInfo parameter)
    {
        return parameter.ParameterType.BaseType is { IsGenericType: true } &&
               parameter.ParameterType.BaseType.GetGenericTypeDefinition() == typeof(EventWithResponseBase<>);
    }

    private async Task HandleEventWithResponseAsync(MethodInfo method, object eventType, Guid eventId, GrainId? contextStorageGrainId)
    {
        if (method.ReturnType.IsGenericType &&
            method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = method.ReturnType.GetGenericArguments()[0];
            if (typeof(EventBase).IsAssignableFrom(resultType))
            {
                try
                {
                    var eventResult = await (dynamic)method.Invoke(this, [eventType])!;
                    var eventWrapper = new EventWrapper<EventBase>(eventResult, eventId, this.GetPrimaryKey(),
                        contextStorageGrainId);
                    await PublishAsync(eventWrapper);
                }
                catch (Exception ex)
                {
                    // TODO: Make this better.
                    Logger.LogError(ex, "Error invoking method {MethodName} with event type {EventType}", method.Name,
                        eventType.GetType().Name);
                }
            }
            else
            {
                var errorMessage =
                    $"The event handler of {eventType.GetType()}'s return type needs to be inherited from EventBase.";
                Logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
        else
        {
            var errorMessage =
                $"The event handler of {eventType.GetType()} needs to have a return value.";
            Logger.LogError(errorMessage);
            throw new InvalidOperationException(errorMessage);
        }
    }
}