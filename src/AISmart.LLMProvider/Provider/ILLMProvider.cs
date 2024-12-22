namespace AISmart.LLMProvider.Provider;

public interface ILLMProvider<T>
{
    Task<T?> SendAsync(string message);
    
    Task<T?> SendAsync(string message, List<T> history);
    
    Task<T?> SendAsync(string message, List<T> history, string? description);
    
}
