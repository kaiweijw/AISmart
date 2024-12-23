
        var xAgentState = await xGAgent.GetStateAsync();
        xAgentState.ThreadIds.Count.ShouldBe(1);

        var investmentAgentState = await investmentGAgent.GetStateAsync();
        investmentAgentState.Content.Count.ShouldBe(1);

        var developerAgentState = await developerGAgent.GetStateAsync();
        developerAgentState.Content.Count.ShouldBe(1);
    }

    [Fact]
    public async Task SendTransactionTest()
    {
        const string chainId = "AELF";
        const string senderName = "Test";
        var createTransactionEvent = new CreateTransactionEvent
        {
            ChainId = chainId,
            SenderName = senderName,
            ContractAddress = "JRmBduh4nXWi1aXgdUsj5gJrzeZb2LxmrAbf7W99faZSvoAaE",
            MethodName = "Transfer",
        };
        var guid = Guid.NewGuid();
        var aelfGAgent = await Silo.CreateGrainAsync<AElfGAgent>(guid);
        var txGrain = await Silo.CreateGrainAsync<TransactionGrain>(guid);
        Silo.AddProbe<ITransactionGrain>(_ => txGrain);
        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        Silo.AddProbe<IPublishingGAgent>(_ => publishingAgent);

        await aelfGAgent.ExecuteTransactionAsync(createTransactionEvent);

        var aelfGAgentState = await aelfGAgent.GetAElfAgentDto();
        aelfGAgentState.PendingTransactions.Count.ShouldBe(1);
    }
    
    [Fact]
    public async Task ReceiveMessageTest()
    {
       
        var guid = Guid.NewGuid();
        var groupAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var telegramGAgent = await Silo.CreateGrainAsync<TelegramGAgent>(guid);
        await groupAgent.Register(telegramGAgent);
        var txGrain = await Silo.CreateGrainAsync<TelegramGrain>(guid);
        Silo.AddProbe<ITelegramGrain>(_ => txGrain);
        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        await publishingAgent.PublishTo(groupAgent);
        Silo.AddProbe<IPublishingGAgent>(_ => publishingAgent);
        await publishingAgent.PublishEventAsync(new ReceiveMessageEvent
        {
            MessageId = "11",
            ChatId = "12",
            Message = "Test",
            BotName = "Test"
        });
    }
    
    [Fact]
    public async Task SendMessageTest()
    {
        var guid = Guid.NewGuid();
        var groupAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var telegramGAgent = await Silo.CreateGrainAsync<TelegramGAgent>(guid);
        await groupAgent.Register(telegramGAgent);
        var txGrain = await Silo.CreateGrainAsync<TelegramGrain>(guid);
        Silo.AddProbe<ITelegramGrain>(_ => txGrain);
        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        await publishingAgent.PublishTo(groupAgent);
        Silo.AddProbe<IPublishingGAgent>(_ => publishingAgent);
        await publishingAgent.PublishEventAsync(new SendMessageEvent
        {
            ChatId = "12",
            Message = "bot message",
            BotName ="Test",
            ReplyMessageId = "11"
        });
    }
    
    [Fact]
    public async Task PumpFunReceiveMessageTest()
    {
        var guid = Guid.NewGuid();
        var groupAgent = await Silo.CreateGrainAsync<GroupGAgent>(Guid.NewGuid());
        var pumpFunGAgent = await Silo.CreateGrainAsync<PumpFunGAgent>(guid);
        await groupAgent.Register(pumpFunGAgent);
        var txGrain = await Silo.CreateGrainAsync<PumpFunGrain>(guid);
        Silo.AddProbe<IPumFunGrain>(_ => txGrain);
        var publishingAgent = await Silo.CreateGrainAsync<PublishingGAgent>(guid);
        await publishingAgent.PublishTo(groupAgent);
        Silo.AddProbe<IPublishingGAgent>(_ => publishingAgent);
        await publishingAgent.PublishEventAsync(new PumpFunReceiveMessageEvent()
        {
            ReplyId = "11",
            ChatId = "12",
            RequestMessage = "Test"
        });
    }
}