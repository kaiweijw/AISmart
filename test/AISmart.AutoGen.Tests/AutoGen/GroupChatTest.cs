using AISmart.AutoGen.Tests.Common;
using AutoGen;
using AutoGen.Anthropic.DTO;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;

namespace AISmart.AutoGen.Tests;

public class GroupChatTest : AISmartAutoGenTestBase
{
    public GroupChatTest()
    {
    }

    public async Task Group_Chat_Test()
    {
        var client = GetRequiredService<ChatClient>();

        #region agent define

        IAgent groupAdmin = new OpenAIChatAgent(
                chatClient: client,
                name: "groupAdmin",
                systemMessage: "You are the admin of the group chat")
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        IAgent adminAgent = new OpenAIChatAgent(chatClient: client, name: "admin", """
                You are a manager who solves user problems by organizing agents,
                Here's available agents who you can assign task to:
                - aelf blockchain: handle aelf blockchain transaction
                - telegram: handle telegram request
                - dex: support crypto coin buy and sell and price

                The workflow is as follows:
                - You take the problem from user.
                - You break the problem into small tasks.
                - Once a small task is resolved, you summarize the completed steps and create the next step.
                - You repeat the above steps until the coding problem is resolved.

                You can use the following json format to assign task to agents:
                ```task
                {
                    "to": "{agent_name}",
                    "task": "{a short description of the task}",
                    "context": "{previous context from scratchpad}"
                }
                ```

                If you need to ask user for extra information, you can use the following format:
                ```ask
                {
                    "question": "{question}"
                }
                ```
                """)
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        #region AelfBlockChain Agent

        var aelfBlockChain = new AelfBlockChain();
        var aelfBlockChainFunctionMap = new Dictionary<string, Func<string, Task<string>>>()
        {
            { nameof(aelfBlockChain.BuyToken), aelfBlockChain.BuyTokenWrapper },
        };
        var aelfBlockChainMiddleware =
            new FunctionCallMiddleware(new[] { aelfBlockChain.BuyTokenContract }, aelfBlockChainFunctionMap);
        IAgent aelfBlockChainAgent =
            new OpenAIChatAgent(chatClient: client, name: "aelf blockchain",
                    systemMessage:
                    "you are the aelf blockchain handler agent. you can send or receive transaction from aelf chain")
                .RegisterMessageConnector()
                .RegisterStreamingMiddleware(aelfBlockChainMiddleware)
                .RegisterPrintMessage();

        #endregion

        #region Twitter BlockChain Agent

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
        IAgent twitterAgent = new OpenAIChatAgent(chatClient: client, name: "twitter",
                systemMessage: "you are twitter AI assistant")
            .RegisterMessageConnector()
            .RegisterStreamingMiddleware(twitterMiddleware)
            .RegisterPrintMessage();

        #endregion

        #region Dex exchange Agent

        IAgent dexAgent = new OpenAIChatAgent(chatClient: client, name: "dex exchange",
                systemMessage:
                "you are dex exchange AI assistant, you can get crypto coin price and buy or sell crypto coin")
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        IAgent userAgent = new DefaultReplyAgent(name: "user", defaultReply: GroupChatExtension.TERMINATE)
            .RegisterPrintMessage();

        #endregion

        #endregion

        #region agent transition define

        var adminToDexAgent = Transition.Create(adminAgent, dexAgent);
        var adminToTwitterAgent = Transition.Create(adminAgent, twitterAgent);
        var adminToAelfChainBlockAgent = Transition.Create(adminAgent, aelfBlockChainAgent);
        var telegramToAelfBlockChain = Transition.Create(twitterAgent, aelfBlockChainAgent);

        var userToAdmin = Transition.Create(userAgent, adminAgent);
        var adminToUser = Transition.Create(adminAgent, userAgent);

        var workflow = new Graph([
            adminToDexAgent, adminToTwitterAgent, adminToAelfChainBlockAgent, telegramToAelfBlockChain, userToAdmin,
            adminToUser
        ]);

        #endregion

        #region group define

        var groupChat = new GroupChat(
            admin: groupAdmin,
            members: [adminAgent, dexAgent, aelfBlockChainAgent, userAgent, twitterAgent],
            workflow: workflow);

        # endregion

        #region user call

        var task =
            "get aelf-usdt symbol price from dex exchange, if the price is over 1$, send the current price to Twitter and buy elf token from aelf chain";
        var chatHistory = new List<IMessage>
        {
            new TextMessage(Role.Assistant, task)
            {
                From = userAgent.Name
            }
        };

        await foreach (var message in groupChat.SendAsync(chatHistory, maxRound: 10))
        {
            if (message.From == adminAgent.Name)
            {
                // Task complete!
                break;
            }
        }

        #endregion
    }
}