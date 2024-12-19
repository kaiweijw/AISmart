using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agents;
using AISmart.Agents.Group;
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
    private static readonly Guid _publishId = Guid.NewGuid();

    public PumpFunChatService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }
    
    public async Task ReceiveMessagesAsync(PumpFunInputDto inputDto)
    {
        // To filter only messages that mention the bot, check if message.Entities.type == "mention".
        // Group message auto-reply, just add the bot as a group admin.
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

    // 单独提前初始化，不应该放在主业务里边
    public async Task SetGroupsAsync(string chatId)
    {
        // TODO agentId? zhifeng
        var groupAgent = _clusterClient.GetGrain<IStateGAgent<GroupAgentState>>(GuidHelper.UniqGuid());
        var pumpFunGAgent = _clusterClient.GetGrain<IStateGAgent<PumpFunGAgent>>(Guid.NewGuid());
        var autogenAgent=  _clusterClient.GetGrain<IAutogenGAgent>(Guid.NewGuid());
        // TODO:新的方法，setChatId，在interface实现一个方法，把元数据都放到state里边，看最新的dev的telegramService
        pumpFunGAgent
        // TODO:
        autogenAgent.RegisterAgentEvent(typeof(PumpFunGAgent), [typeof(PumpFunReceiveMessageEvent), typeof(PumpFunSendMessageEvent)]);
        
        await groupAgent.Register(autogenAgent);
        await groupAgent.Register(pumpFunGAgent);
        
        var publishingAgent = _clusterClient.GetGrain<IPublishingGAgent>(_publishId);
        await publishingAgent.PublishTo(groupAgent);
    }
    
    public Task<PumFunResponseDto> SearchAnswerAsync(string replyId)
    {
        // TODO:get replyMessage by replyId, depends on CQRS
        var content = "";
        // TODO: store ReplyId-requestMessageId mapping in db, depends on CQRS integration
        var requestMessageId = "";
        PumFunResponseDto answer = new PumFunResponseDto
        {
            ReplyId = replyId,
            ReplyMessage = content
        };

        return Task.FromResult(answer);
    }
}