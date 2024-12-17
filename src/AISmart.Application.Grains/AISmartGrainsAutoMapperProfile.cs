using AISmart.Application.Grains.Command;
using AISmart.Application.Grains.Dto;
using AISmart.Cqrs.Command;
using AISmart.Domain.Grains.Event;
using AutoMapper;

namespace AISmart.Application.Grains;

public class AISmartGrainsAutoMapperProfile : Profile
{
    public AISmartGrainsAutoMapperProfile()
    {
        // User AutoMap
        CreateMap<BasicEvent, TelegramEvent>().ReverseMap();
        CreateMap<AgentTaskState, AgentTaskDto>().ReverseMap();
        CreateMap<SaveStateCommand, CreateTransactionEventIndex>();

    }
}