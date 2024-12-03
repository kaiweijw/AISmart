using System;


namespace AISmart.Agents;

public class Event
{
    // The type of the event, e.g., "UserSignup", "ErrorOccurred"
    public string EventType { get; set; }

    // The time when the event occurred
    public DateTime EventTime { get; set; }

    // The main content or data related to the event
    public string Data { get; set; }

    // A unique identifier for the event
    public Guid EventId { get; set; }

    // The source of the event (optional)
    public string Source { get; set; }

    // Constructor to initialize the event
    public Event(string eventType, string data, string source = "")
    {
        EventType = eventType;
        EventTime = DateTime.UtcNow; // Use UTC time for event timestamp
        Data = data;
        EventId = Guid.NewGuid(); // Generate a unique ID
        Source = source;
    }

    public Event()
    {
        
    }

    // Method to output event information
    public override string ToString()
    {
        return $"EventId: {EventId}\nEventType: {EventType}\nEventTime: {EventTime}\nData: {Data}\nSource: {Source}";
    }
}