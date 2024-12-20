using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.CQRS.Provider;
using AISmart.Dto;
using AISmart.Events;
using AISmart.GAgent.Autogen;
using AISmart.PumpFun;
using AISmart.Sender;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Service;

public class PumpFunChatService :  ApplicationService, IPumpFunChatService
{
    private readonly IClusterClient _clusterClient;
    private readonly ICQRSProvider _cqrsProvider;

    private static readonly Guid _publishId = Guid.NewGuid();

    // TODO:how to initiate
    public PumpFunChatService(IClusterClient clusterClient, ICQRSProvider cqrsProvider)
    {
        _clusterClient = clusterClient;
        _cqrsProvider = cqrsProvider;
    }
    
    public async Task ReceiveMessagesAsync(PumpFunInputDto inputDto)
    {
        if (inputDto.RequestMessage != null)
        {
            var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(_publishId);

            await  publishingAgent.PublishEventAsync(new PumpFunReceiveMessageEvent
            {
                ReplyId = inputDto.ReplyId,
                RequestMessage = inputDto.RequestMessage,
            });
        }
    }

    public async Task SetGroupsAsync(string chatId, string botName)
    {
        var groupAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(GuidHelper.UniqGuid());
        var pumpFunGAgent = _clusterClient.GetGrain<IPumpFunGAgent>(Guid.NewGuid());
        // TODO: the second param need?
        await pumpFunGAgent.SetPumpFunConfig(chatId, botName);
        var autogenAgent=  _clusterClient.GetGrain<IAutogenGAgent>(Guid.NewGuid());
        
        autogenAgent.RegisterAgentEvent(typeof(PumpFunGAgent), [typeof(PumpFunReceiveMessageEvent), typeof(PumpFunSendMessageEvent)]);
        
        await groupAgent.Register(autogenAgent);
        await groupAgent.Register(pumpFunGAgent);
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(_publishId);
        await publishingAgent.PublishTo(groupAgent);
    }
    
    public async Task<PumFunResponseDto> SearchAnswerAsync(string replyId)
    {
        // TODO:get replyMessage by replyId, depends on CQRS
        // 获取PumpFunGAgentState
        var pumpFunGAgent = _clusterClient.GetGrain<IPumpFunGAgent>(Guid.NewGuid());
        var grainId =  _clusterClient.GetGrain<IPumpFunGAgent>(guid).GetGrainId();
        
        var stateResult = await _cqrsProvider.QueryAsync("test", grainId.ToString());
        var state = await pumpFunGAgent.GetStateAsync();
        
        // TODO: store ReplyId-requestMessageId mapping in db, depends on CQRS integration
        PumFunResponseDto answer = new PumFunResponseDto
        {
            ReplyId = replyId,
            ReplyMessage = state.PendingMessages[replyId];
        };

        return await Task.FromResult(answer);
    }
}