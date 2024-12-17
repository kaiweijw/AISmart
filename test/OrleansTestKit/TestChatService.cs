using AISmart.GAgent.Autogen.Applications;
using AutoGen.Core;
using Volo.Abp.DependencyInjection;

namespace Orleans.TestKit;

public class TestChatService : IChatService, ITransientDependency
{
    public async Task<IMessage> SendAsync(string agentName, string message, IEnumerable<IMessage> chatHistory)
    {
        string jsonResponse = @"{
    ""compete"": ""This is a mock completion message.""
}";
        var response = new Message(Role.System, jsonResponse);
        return response;
    }
}