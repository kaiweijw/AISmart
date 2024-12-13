using System.Text.Json;
using System.Text.Json.Serialization;
using AutoGen.Core;

namespace AISmart.AutoGen.Tests.Common;

public class GroupChatMiddleware
{
    /// <summary>
    /// handle middleware event
    /// </summary>
    /// <param name="eventName">event name</param>
    /// <param name="parameters">event parameter json string</param>
    /// <returns></returns>
    [Function]
    public async Task<string> HandleEventAsync(string eventName, string parameters)
    {
        if (eventName == "DexExchange")
        {
            return "get 10 Elf";
        }
        else
        {
            return "you have send a twitter";
        }
        
        // return $"{eventName}---{parameters}";
    }

    public FunctionContract HandleEventAsyncContract
    {
        get => new FunctionContract
        {
            ClassName = nameof(GroupChatMiddleware),
            Name = nameof(HandleEventAsync),
            Description = @"Handle Event",
            ReturnType = typeof(Task<string>),
            ReturnDescription = "handle event result",
            Parameters = new global::AutoGen.Core.FunctionParameterContract[]
            {
                new FunctionParameterContract
                {
                    Name = @"eventName",
                    Description = @"event name",
                    ParameterType = typeof(string),
                    IsRequired = true,
                },
                new FunctionParameterContract()
                {
                    Name = @"parameters",
                    Description = @"event parameters",
                    ParameterType = typeof(string),
                    IsRequired = true,
                }
            },
        };
    }
    
    
    public Task<string> HandleEventAsyncWrapper(string arguments)
    {
        var schema = JsonSerializer.Deserialize<HandleEventAsyncSchema>(
            arguments,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            });

        return HandleEventAsync(schema.EventName, schema.Parameters);
    }
    
    private class HandleEventAsyncSchema
    {
        [JsonPropertyName(@"eventName")] public string EventName { get; set; }
        [JsonPropertyName(@"parameters")] public string Parameters { get; set; }
    }
}