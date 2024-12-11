using System;
using Orleans.Runtime;

namespace AISmart.Application.Grains;

public class EventWrapper<T>
{
    // Properties with getters and setters
    public T Event { get; private set; }
    public StreamId StreamId { get; private set; }
    public GrainId GrainId { get; private set; }

    public int Count { get; set; } = 0;
    
    public int SubscriberCount { get; set; } = 0;
    
    public Boolean Success { get; set; }

    // Constructor
    public EventWrapper(T @event, StreamId streamId, GrainId grainId,int subscriberCount)
    {
        Event = @event;
        StreamId = streamId;
        GrainId = grainId;
        SubscriberCount = subscriberCount;
        Success = false;
        
    }

    // Optionally, you can add methods or other functionality as needed
}