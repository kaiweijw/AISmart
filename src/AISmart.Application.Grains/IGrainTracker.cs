using System.Collections.Concurrent;
using AISmart.Agents;
using AISmart.Application.Grains.Agents.Developer;
using AISmart.Application.Grains.Agents.MarketLeader;
using AISmart.Application.Grains.Agents.X;

namespace AISmart.Application.Grains;

public interface IGrainTracker
{
    
}

public static class GrainTracker
{
    public static ConcurrentQueue<IAgent<XAgentState>> XAgents = new();
    public static ConcurrentQueue<IAgent<MarketLeaderAgentState>> MarketLeaderAgents = new();
    public static ConcurrentQueue<IAgent<DeveloperAgentState>> DeveloperAgents = new();
}