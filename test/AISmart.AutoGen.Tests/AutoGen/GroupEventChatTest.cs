using AISmart.AutoGen.Tests.Common;
using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI.Chat;

namespace AISmart.AutoGen.Tests;

public class GroupEventChatTest : AISmartAutoGenTestBase
{
    public GroupEventChatTest()
    {
    }

    [Fact]
    public async Task Group_Event_Chant_Test()
    {
        var client = GetRequiredService<ChatClient>();

        #region agent defines

        var groupChatMiddleware = new GroupChatMiddleware();
        var groupChatContract = new List<FunctionContract>()
        {
            groupChatMiddleware.HandleEventAsyncContract
        };

        var groupChatFunctionMap = new Dictionary<string, Func<string, Task<string>>>()
        {
            { nameof(groupChatMiddleware.HandleEventAsync), groupChatMiddleware.HandleEventAsyncWrapper }
        };

        var functionMiddleware = new FunctionCallMiddleware(groupChatContract, groupChatFunctionMap);

        IAgent adminAgent = new OpenAIChatAgent(chatClient: client, name: "admin", """
                You are a manager who solves user problems by organizing agents,
                - The events and their parameters are described as follows Json:
                {
                    EventName:"Twitter",
                    EventDescription:"Handle Twitter Event",
                    EventParameters:[{
                        ParameterType:String,
                        ParameterName:"Content",
                        ParameterDescription:"twitter content",
                        ParameterIsRequired:true,
                    }]
                }
                {
                    EventName:"DexExchange",
                    EventDescription:"Handle Decentralized Event",
                    EventParameters:[{
                        ParameterType:String,
                        ParameterName:"Symbol",
                        ParameterDescription:"Token symbol",
                        ParameterIsRequired:true,
                    },{
                        ParameterType:float,
                        ParameterName:"Amount",
                        ParameterDescription:"The number of tokens to purchase",
                        ParameterIsRequired:true,
                    }
                    ]
                }

                - You can understand what the event can do through the event description, and know the type, name, and description of each parameter through the event parameters. 
                If the event description can handle the user's request, you need to assemble the request into the following JSON format:
                {
                    EventName:""
                    Parameters:""
                }
                JSON explanation: EventName is the name of the event, and Parameters is a JSON string consisting of the parameter names and values corresponding to the event.

                The workflow is as follows:
                - You take the problem from user.
                - Split the task into different events, and use the output of the previous event combined with the user's request to determine the execution of the next event.
                - Based on the event's response, reorganize the next request in line with the user's intent until the user's issue is resolved.

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
            .RegisterStreamingMiddleware(functionMiddleware)
            .RegisterPrintMessage();
        
        
        var task =
            """
            Purchase 10 ELF on a decentralized exchange, and then post a Twitter message after the purchase.
            """;
        var chatHistory = new List<IMessage>
        {
            new TextMessage(Role.User, task)
            {
                From = adminAgent.Name
            }
        };
        
        var dexResponse = await adminAgent.SendAsync("What should be done next?", chatHistory);
        chatHistory.Add(dexResponse);
        var twitterResponse = await adminAgent.SendAsync("What should be done next?", chatHistory);
        chatHistory.Add(twitterResponse);

        #endregion
    }
}