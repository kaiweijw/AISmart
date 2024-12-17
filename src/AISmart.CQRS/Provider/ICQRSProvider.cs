using System.Threading.Tasks;
using AISmart.Agents;

namespace AISmart.CQRS.Provider;

public interface ICQRSProvider
{
    Task PublishAsync(BaseState state, string id);
    
    Task<BaseState> QueryAsync(string index, string id);
}