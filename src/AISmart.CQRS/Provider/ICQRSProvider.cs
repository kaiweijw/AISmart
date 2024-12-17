using System.Threading.Tasks;
using AISmart.Agents;

namespace AISmart.CQRS.Provider;

public interface ICQRSProvider
{
    Task Publish(BaseState state, string id);
}