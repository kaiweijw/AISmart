using System.Threading.Tasks;
using AISmart.Cqrs.Command;

namespace AISmart.Cqrs;

public interface ICqrsProvider
{
    Task Publish(BaseState state, string id);
}