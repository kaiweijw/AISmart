using System.Threading.Tasks;
using AISmart.Dto;

namespace AISmart.Provider;

public interface ITelegramProvider
{
    public Task<string> GetUpdatesAsync(string sendUser);
    public Task TestUpdatesMessagesAsync(TelegramUpdateDto updateMessage);
    public Task SendMessageAsync(string sendUser, string chatId, string message, ReplyParamDto? replyParam = null);
    public Task SendPhotoAsync(string sendUser, PhotoParamsDto photoParams);
}