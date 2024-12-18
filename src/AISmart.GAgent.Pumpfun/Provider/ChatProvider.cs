using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Agents.X.Events;
using AISmart.Dto;
using AISmart.Sender;
using Orleans;

namespace AISmart.Provider;

public class ChatProvider : IChatProvider
{
    
    private readonly IClusterClient _clusterClient;
    
    private readonly IHttpClientFactory _httpClientFactory;

    public ChatProvider(IHttpClientFactory httpClientFactory, IClusterClient clusterClient)
    {
        _httpClientFactory = httpClientFactory;
        _clusterClient = clusterClient;
    }
    
    public async Task<AskOutputDto> AskAsync(AskInputDto askInputDto)
    {
        var groupAgentId = askInputDto.agentId;
        
        // GetGrain by id
        var groupStateGAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(Guid.Parse(groupAgentId));
        
        var publishingGAgent = _clusterClient.GetGrain<IPublishingGAgent>(Guid.NewGuid());
        
        // register agent
        await publishingGAgent.PublishTo(groupStateGAgent);
        
        var replyId = Guid.NewGuid().ToString();
        var xThreadCreatedEvent = new XThreadCreatedEvent
        {
            Id = replyId,
            Content = askInputDto.RequestMessage
        };
        
        // send message, autogen receive
        await publishingGAgent.PublishEventAsync(xThreadCreatedEvent);
        
        // TODO: store ReplyId-requestMessageId mapping in db, depends on CQRS integration
        
        // return response
        var response = new AskOutputDto()
        {
            ReplyId = replyId
        };

        return response;
    }

    public async Task AnswerAsync(SearchAnswerOutputDto searchAnswerOutput)
    {
        // TODO:get replyMessage by replyId, depends on CQRS
        var content = "";
        
        // appSettings 
        var path = "/api/app/chat/callback";
        
        // http request
        var client = _httpClientFactory.CreateClient("MyRemoteService");

        // Send the POST request
        // var response = await client.PostAsync("/api/example/data", content);
        // response.EnsureSuccessStatusCode();
    }
    
    public Task<SearchAnswerOutputDto> SearchAnswerAsync(string replyId)
    {
        // TODO:get replyMessage by replyId, depends on CQRS
        var content = "";
        // TODO: store ReplyId-requestMessageId mapping in db, depends on CQRS integration
        var requestMessageId = "";
        SearchAnswerOutputDto answerOutputDto = new SearchAnswerOutputDto();
        answerOutputDto.ReplyId = replyId;
        answerOutputDto.ReplyMessage = content;
        answerOutputDto.RequestMessageId = requestMessageId;
        
        return Task.FromResult(answerOutputDto);
    }
    
}