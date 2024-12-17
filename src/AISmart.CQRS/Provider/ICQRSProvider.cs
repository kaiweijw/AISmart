using System.Threading.Tasks;
using AISmart.Agents;
using AISmart.CQRS.Dto;

namespace AISmart.CQRS.Provider;

public interface ICQRSProvider
{
    Task PublishAsync(BaseState state, string id);
    
    Task<BaseStateIndex> QueryAsync(string index, string id);
}