using System.Collections.Generic;
using System.Threading.Tasks;
using AutoGen.Core;

namespace AISmart.Provider;

public interface IChatAgentProvider
{
    Task<IMessage?> SendAsync(string agentName, string message, IEnumerable<IMessage>? chatHistory);

    void SetAgent(string agentName, string systemMessage);
}
