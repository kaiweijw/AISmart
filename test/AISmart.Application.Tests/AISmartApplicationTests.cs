using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Service;
using Newtonsoft.Json;
using Volo.Abp.Modularity;
using Xunit;

namespace AISmart.Samples;

public abstract class AISmartApplicationTests <TStartupModule> : AISmartApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IRoutingService _routingService;
    private readonly IAgentService _agentService;
    
    protected AISmartApplicationTests()
    {
        _routingService = GetRequiredService<IRoutingService>();
        _agentService = GetRequiredService<IAgentService>();
    }
    
    [Fact]
    public async Task AiFrameWorkTests()
    {
        // 1.1. init AI frameWork compenent
        EventStack eventStack = new EventStack();
        // 1.2 init AI Aelf Agent
        // split task
        Agent AelfAIagent = await _agentService.CreateAgent(new AgentGoal()
        {
            Description = "split task"
        });
        
        Agent RoutingAgent = await _agentService.CreateAgent(new AgentGoal()
        {
            Description = "Route to Agent"
        });
        
        Agent EvaluatorAgent = await _agentService.CreateAgent(new AgentGoal()
        {
            Description = "Evaluate AgentTask"
        });
       
        // 2. recieve Aelf trasnaction
        // 2.1 aelf produce transaction
        var jsonData = new
        {
            TransactionId = "97003ffe519bd9cda5bf80d40d6a944e07a86743a789176676d0e394a43eb847",
            Status = "MINED",
            Logs = new object[] { },
            Bloom = "AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA==",
            BlockNumber = 152184013,
            BlockHash = "a05e595ccf111d451b6542a9a64d86928492fac68cbc80d9437028c648c957de",
            Transaction = new
            {
                From = "2htmQUMEyrgLi1fMMuMycDquJTAEDbBKcG8L6ZT3HbxjvsTGYe",
                To = "ASh2Wt7nSEmYqnGxPPzp4pnVDU4uhj1XW9Se5VeZcX2UDdyjx",
                RefBlockNumber = 152184012,
                RefBlockPrefix = "BRI7Ww==",
                MethodName = "DonateResourceToken",
                Params = "{ \"blockHash\": \"05123b5b86a6ec3a5a72b20421b8bfd0f07c4b0d61031f8cb3078e52de2d220e\", \"blockHeight\": \"152184012\" }",
                Signature = "fd+Tc8a3UOshWr5/1EaoJEmBR5LHNXRrFrzmnLRAigJAu3fY1CA2AtIuOZMuR16xsnk+ANjazYjX6SNwYY1orQE="
            },
            ReturnValue = "",
            Error = (string)null,
            TransactionSize = 216
        };
        
        
        string serializedData = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

        // 2.2 
        Event aelfEvent = new Event()
        {
            EventId = Guid.NewGuid(),  // 
            EventType = "BlockMined", 
            EventTime = DateTime.UtcNow, //
            Data = serializedData,
            Source = "Aelf"
        };
        // 2.3 Al-framework receive aelfEvent
        eventStack.Push(aelfEvent);


        // 3.1 process Aelf event
        eventStack.Pop(out Event poppedEvent);

        if (poppedEvent.Source =="Aelf")
        {
            // 3.2 Task intent；
            List<AgentTask> AgentTaskList = _routingService.SplitAgentTask(AelfAIagent.Id,poppedEvent);

            foreach (var agentTask in AgentTaskList)
            {
                Event agentTaskEvent = new Event()
                {
                    EventId = Guid.NewGuid(),  // 
                    EventType = "TaskCreated", 
                    EventTime = DateTime.UtcNow, //
                    Data = agentTask.ToString(),
                    Source = "AelfAIagents"
                };
                eventStack.Push(agentTaskEvent);
            }
        }
        

        // 4.1 process Aelf event
        eventStack.Pop(out Event poppedEvent2);
        
        // 4.2 relate content find wallet Agent
        Agent WalletAgent = _routingService.RoutingAgent(RoutingAgent.Id,poppedEvent2);
        // 4.3 do wallet action
        AgentTask walletAgentTask = await _routingService.InvokeAgent(WalletAgent.Id,poppedEvent2);

        // 4.4 evaluate Result
        walletAgentTask =  await _routingService.EvaluateAgent(EvaluatorAgent.Id, walletAgentTask);

        // 4.5 process evaluate Result
        if (walletAgentTask.success)
        {
            
        }
        else
        {
            // if false,  push back into the stack."
            Event walletTaskFailedEvent = new Event()
            {
                EventId = Guid.NewGuid(),  // 
                EventType = "TaskFailed", 
                EventTime = DateTime.UtcNow, //
                Data = serializedData,
                Source = "walletAgent"
            };
            eventStack.Push(walletTaskFailedEvent);
        }
        
        
        // 5.1 process telegram event
        eventStack.Pop(out Event poppedEvent3);
        
        // 4.2 relate content find wallet Agent
        Agent TelegGramAgent = _routingService.RoutingAgent(RoutingAgent.Id,poppedEvent2);
        // 4.3 do Telegram action
        AgentTask TelegramAgentTask = await _routingService.InvokeAgent(TelegGramAgent.Id,poppedEvent2);

        // 4.4 evaluate Result
        TelegramAgentTask =  await _routingService.EvaluateAgent(EvaluatorAgent.Id, TelegramAgentTask);

        // 4.5 process evaluate Result
        if (TelegramAgentTask.success)
        {
            
        }
        else
        {
            // if false,  push back into the stack."
            Event TelegramTaskFailedEvent = new Event()
            {
                EventId = Guid.NewGuid(),  // 
                EventType = "TaskFailed", 
                EventTime = DateTime.UtcNow, //
                Data = serializedData,
                Source = "TelegramAgent"
            };
            eventStack.Push(TelegramTaskFailedEvent);
        }
    }
}
