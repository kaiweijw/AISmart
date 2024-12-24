using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agent.GEvents;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.CQRS.Provider;
using AISmart.Dto;
using AISmart.Events;
using AISmart.GAgent.Autogen;
using AISmart.PumpFun;
using AISmart.Sender;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Service;

public class PumpFunChatService :  ApplicationService, IPumpFunChatService
{
    private readonly IClusterClient _clusterClient;
    private readonly ICQRSProvider _cqrsProvider;
    private readonly ILogger<PumpFunChatService> _logger;

    public PumpFunChatService(IClusterClient clusterClient, ICQRSProvider cqrsProvider, ILogger<PumpFunChatService> logger)
    {
        _clusterClient = clusterClient;
        _cqrsProvider = cqrsProvider;
        _logger = logger;
    }
    
    public async Task ReceiveMessagesAsync(PumpFunInputDto inputDto)
    {
        _logger.LogInformation("ReceiveMessagesAsync agentId:" + inputDto.AgentId);
        if (inputDto is { RequestMessage: not null, AgentId: not null })
        {
            _logger.LogInformation("ReceiveMessagesAsync2 agentId:" + inputDto.AgentId);
            var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(Guid.Parse(inputDto.AgentId));

            _logger.LogInformation("ReceiveMessagesAsync3 agentId:" + inputDto.AgentId);
            await  publishingAgent.PublishEventAsync(new PumpFunReceiveMessageEvent
            {
                ReplyId = inputDto.ReplyId,
                RequestMessage = inputDto.RequestMessage,
            });
        }
    }

    public async Task<string> SetGroupsAsync(string chatId)
    {
        _logger.LogInformation("SetGroupsAsync, chatId:{chatId}", chatId);
        Guid groupAgentId = Guid.NewGuid();
        var groupAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(groupAgentId);
        var pumpFunGAgent = _clusterClient.GetGrain<IPumpFunGAgent>(Guid.NewGuid());
        _logger.LogInformation("SetGroupsAsync2, chatId:{chatId}", chatId);
        await pumpFunGAgent.SetPumpFunConfig(chatId);
        var autogenAgent=  _clusterClient.GetGrain<IAutogenGAgent>(Guid.NewGuid());
        
        _logger.LogInformation("SetGroupsAsync3, chatId:{chatId}", chatId);
        autogenAgent.RegisterAgentEvent(typeof(PumpFunGAgent), [typeof(PumpFunReceiveMessageEvent), typeof(PumpFunSendMessageEvent)]);
        
        await groupAgent.RegisterAsync(autogenAgent);
        await groupAgent.RegisterAsync(pumpFunGAgent);
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(groupAgentId);
        _logger.LogInformation("SetGroupsAsync3, groupAgentId:{groupAgentId}", JsonConvert.SerializeObject(publishingAgent));
        await publishingAgent.PublishToAsync(groupAgent);

        return groupAgentId.ToString();
    }
    
    public async Task<PumFunResponseDto> SearchAnswerAsync(string replyId)
    {
        _logger.LogInformation("SearchAnswerAsync, replyId:{replyId}", replyId);
        var grainId =  _clusterClient.GetGrain<IPumpFunGAgent>(Guid.Parse(replyId)).GetGrainId();
        // get PumpFunGAgentState
        var stateResult = await _cqrsProvider.QueryAsync("pumpfungagentstateindex", grainId.ToString());
        _logger.LogInformation("SearchAnswerAsync, stateResult:{stateResult}", JsonConvert.SerializeObject(stateResult));
        var state = stateResult.State;
        PumpFunGAgentState? pumpFunGAgentState = JsonConvert.DeserializeObject<PumpFunGAgentState>(state);
        PumFunResponseDto answer = new PumFunResponseDto
        {
            ReplyId = pumpFunGAgentState.responseMessage[replyId].ReplyId,
            ReplyMessage = pumpFunGAgentState.responseMessage[replyId].ReplyMessage
        };
        _logger.LogInformation("SearchAnswerAsync3, replyId:{replyId}", replyId);
        return await Task.FromResult(answer);
    }
}