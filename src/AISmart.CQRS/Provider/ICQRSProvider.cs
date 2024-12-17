using System.Threading.Tasks;
using AISmart.Cqrs.Command;

namespace AISmart.CQRS.Provider;

public interface ICQRSProvider
{
    Task Publish(BaseState state, string id);
}