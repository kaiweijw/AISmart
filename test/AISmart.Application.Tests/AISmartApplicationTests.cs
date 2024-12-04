using System.Net;
using System.Runtime.Intrinsics.X86;
using System.Threading.Tasks;
using AISmart.Agents.Service;
using Volo.Abp.Modularity;

namespace AISmart;

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
    
    /*[Fact]
    public async Task test()
    {
       
        // 1. event 
        // 
        // 2. create AI agent;  Task intent；
        Agents AIagents = _agentService.CreateAgent();
        // 3. Task intent；
        List<Task> Tasks _routingService.InvokeAgent(AIagents.id,event.content);

        // 4. resolve task Dependency
        for Task in Tasks:
            
            Content = Pop(Content)
            Content = _routingService.InvokeAgent(AIagents.id,Task.content);
            // 4.2 relate content find Agent
            // wallet
            _agentService.CreateAgent(); 
            // do wallet action
            Content = _routingService.InvokeAgent();
            
            push(Content)
                
                
            
            Content = Pop(Content)
            Content = _routingService.InvokeAgent(AIagents.id,Task.content)

            // 4.3 relate task content find Agent
            // pre relate lasttask execute Task，relte Task result
            // create telegram 
            _agentService.CreateAgent(); 
            // notice telegram
            _routingService.InvokeAgent()   
            push(Content)     
                
                
    }*/
}
