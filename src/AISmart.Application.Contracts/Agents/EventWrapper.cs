using System;
using Orleans.Runtime;

namespace AISmart.Application.Grains;

public class EventWrapper<T>
{
    // Properties with getters and setters
    public T Event { get; private set; }
    public StreamId StreamId { get; private set; }
    public GrainId GrainId { get; private set; }

    public int count { get; set; } = 0;

    // Constructor
    public EventWrapper(T @event, StreamId streamId, GrainId grainId)
    {
        Event = @event;
        StreamId = streamId;
        GrainId = grainId;
    }

    // Optionally, you can add methods or other functionality as needed
}