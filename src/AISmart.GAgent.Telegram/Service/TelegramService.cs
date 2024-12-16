using System;
using System.Threading.Tasks;
using AISmart.Agent;
using AISmart.Agents;
using AISmart.Agents.Group;
using AISmart.Dto;
using AISmart.Events;
using AISmart.Sender;
using Orleans;
using Volo.Abp.Application.Services;

namespace AISmart.Service;

public class TelegramService :  ApplicationService,ITelegramService
{
    private readonly IClusterClient _clusterClient;

    public TelegramService(IClusterClient clusterClient)
    {
        _clusterClient = clusterClient;
    }
    
    public async Task ReceiveMessagesAsync(TelegramUpdateDto updateMessage)
    {
        // To filter only messages that mention the bot, check if message.Entities.type == "mention".
        // Group message auto-reply, just add the bot as a group admin.
        if (updateMessage.Message != null)
        {
            var publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
           
            await  publishingAgent.PublishEventAsync(new ReceiveMessageEvent
            {
                MessageId = updateMessage.Message.MessageId.ToString(),
                ChatId = updateMessage.Message.Chat.Id.ToString(),
                Message = updateMessage.Message.Text,
                NeedReplyBotName = "Test"
            });
        }
    }

    public async Task SetGroupsAsync()
    {
        var groupAgent = _clusterClient.GetGrain<IStateAgent<GroupAgentState>>(Guid.NewGuid());
        var telegramAgent = _clusterClient.GetGrain<IStateAgent<TelegramGAgentState>>(Guid.NewGuid());
        await groupAgent.Register(telegramAgent);
        var publishingAgent = _clusterClient.GetGrain<IPublishingAgent>(Guid.NewGuid());
        await publishingAgent.PublishTo(groupAgent);
    }
}