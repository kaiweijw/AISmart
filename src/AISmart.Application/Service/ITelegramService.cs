using System.Threading.Tasks;
using AISmart.Telegram;

namespace AISmart.Service;

public interface ITelegramService
{
    public Task ReceiveMessagesAsync(TelegramUpdateDto updateMessage);
    
    public Task SetGroupsAsync();
}