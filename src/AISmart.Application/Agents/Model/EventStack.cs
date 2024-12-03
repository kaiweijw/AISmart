using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace AISmart.Agents;

public class EventStack
{
    private readonly ConcurrentStack<Event> _stack;

    public EventStack()
    {
        _stack = new ConcurrentStack<Event>();
        Task.Factory.StartNew(ProcessEvents, TaskCreationOptions.LongRunning);
    }

    public void Push(Event eventItem)
    {
        _stack.Push(eventItem);
        Console.WriteLine($"Event pushed: {eventItem.EventId}");
    }

    public bool Pop(out Event eventItem)
    {
        bool success = _stack.TryPop(out eventItem);
        if (success)
        {
            Console.WriteLine($"Event popped: {eventItem.EventId}");
        }
        return success;
    }

    private void ProcessEvents()
    {
        while (true)
        {
            if (Pop(out Event eventItem))
            {
                try
                {
                    Console.WriteLine($"Processing Event:\n{eventItem}");
                    // Place here any processing logic you need for the event
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while processing the event: {ex.Message}");
                }
            }
            else
            {
                // No event to process, reduce CPU usage by sleeping for a short time
                Thread.Sleep(100); // Adjust the sleep duration as needed
            }
        }
    }
}
