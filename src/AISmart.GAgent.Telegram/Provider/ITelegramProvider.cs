using System.Threading.Tasks;

namespace AISmart.Provider;

public interface ITelegramProvider
{
    public  Task<string> GetUpdatesAsync(string sendUser);
    public  Task SendMessageAsync(string sendUser,string chatId, string message);
}