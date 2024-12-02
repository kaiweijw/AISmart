using Orleans.TestingHost;
using Volo.Abp.Modularity;
using Xunit.Abstractions;


namespace AISmart;

public abstract class AISmartOrleansTestBase<TStartupModule> : 
    AISmartTestBase<TStartupModule> where TStartupModule : IAbpModule
{

    protected readonly TestCluster Cluster;

    protected AISmartOrleansTestBase(ITestOutputHelper output) : base()
    {
        Cluster = GetRequiredService<ClusterFixture>().Cluster;
    }
}