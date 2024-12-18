using System.Threading.Tasks;
using AISmart.Dto;
using Microsoft.AspNetCore.Mvc;

namespace AISmart.Provider;

public interface IChatProvider
{
    public Task<AskOutputDto> AskAsync(AskInputDto askInputDto);
    
    public Task AnswerAsync(SearchAnswerOutputDto searchAnswerOutput);
    
    public Task<SearchAnswerOutputDto> SearchAnswerAsync(string replyId);
}