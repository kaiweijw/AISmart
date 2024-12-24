namespace AISmart.LLMProvider.Message;

public interface IMessage
{
    string? From { get; set; }
}

public interface IMessage<out T> : IMessage
{
    T Content { get; }
}

/// <summary>
/// The interface for messages that can get text content.
/// This interface will be used by <see cref="MessageExtension.GetContent(IMessage)"/> to get the content from the message.
/// </summary>
public interface ICanGetTextContent : IMessage
{
    public string? GetContent();
}

/// <summary>
/// The interface for messages that can get a list of <see cref="ToolCall"/>
/// </summary>
public interface ICanGetToolCalls : IMessage
{
    public IEnumerable<ToolCall> GetToolCalls();
}

[Obsolete("Use IMessage instead")]
public interface IStreamingMessage
{
    string? From { get; set; }
}

[Obsolete("Use IMessage<T> instead")]
public interface IStreamingMessage<out T> : IStreamingMessage
{
    T Content { get; }
}
