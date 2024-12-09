using AISmart.AutoGen.Tests.Common;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace AISmart.AutoGen.Tests;

public class AgentFunctionCallTest:AISmartAutoGenTestBase
{
    private readonly ChatClient _chatClient;
    public AgentFunctionCallTest()
    {
        IConfiguration configuration = GetRequiredService<IConfiguration>();
        var apiKey = configuration.GetSection("Chat:APIKey").Value;
        var modelId = configuration.GetSection("Chat:Model").Value;
        _chatClient = new OpenAIClient(apiKey).GetChatClient(modelId);
    }

    [Fact]
    public async Task Function_Call_Test()
    {
        var twitter = new Twitter();

        var twitterContract = new List<FunctionContract>()
        {
            twitter.GetTwitterNewsContract,
            twitter.SendTwitterMessageContract
        };

        var functionMap = new Dictionary<string, Func<string, Task<string>>>()
        {
            { nameof(twitter.GetLastNews), twitter.GetLastNewsWrapper },
            { nameof(twitter.SendTwitterMessage), twitter.SendTwitterMessageWrapper }
        };

        var twitterMiddleware = new FunctionCallMiddleware(twitterContract, functionMap);

        IAgent telegramAgent = new OpenAIChatAgent(
                chatClient: _chatClient,
                name: "twitter agent",
                systemMessage: "You are twitter AI assistant")
            .RegisterMessageConnector()
            .RegisterStreamingMiddleware(twitterMiddleware)
            .RegisterPrintMessage();
        
        var newsResponse = await telegramAgent.SendAsync("what is the last news in twitter");
        newsResponse.GetContent()?.Should().Be("BTC 10W");

        var sendTwitterResponse = await telegramAgent.SendAsync("send \"Woo BTC 10w$\" to Twitter");
        sendTwitterResponse.GetContent()?.Should().Be("Woo BTC 10w$");
    }
}