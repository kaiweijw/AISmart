using System;
using AISmart.Application.Grains;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.TestingHost;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Reflection;

namespace AISmart;

public class ClusterFixture: IDisposable, ISingletonDependency
{
    public ClusterFixture()
    {
        var builder = new TestClusterBuilder();
        builder.AddSiloBuilderConfigurator<TestSiloConfigurations>();
        builder.AddClientBuilderConfigurator<TestClientBuilderConfigurator>();
        Cluster = builder.Build();
        Cluster.Deploy();
    }

    public void Dispose()
    {
        Cluster.StopAllSilos();
    }

    public TestCluster Cluster { get; private set; }
    
    private class TestSiloConfigurations : ISiloConfigurator 
    {
        public void Configure(ISiloBuilder hostBuilder)
        {
           
            hostBuilder.ConfigureServices(services =>
                {
                   

                    services.OnExposing(onServiceExposingContext =>
                    {
                        //Register types for IObjectMapper<TSource, TDestination> if implements
                        var implementedTypes = ReflectionHelper.GetImplementedGenericTypes(
                            onServiceExposingContext.ImplementationType,
                            typeof(IObjectMapper<,>)
                        );
                        services.AddTransient(
                            typeof(IObjectMapper<>),
                            typeof(DefaultObjectMapper<>)
                        );
                        services.AddTransient(
                            typeof(IObjectMapper),
                            typeof(DefaultObjectMapper)
                        );
                        foreach (var type in implementedTypes)
                        {
                            onServiceExposingContext.ExposedTypes.Add(new ServiceIdentifier(type));
                        }
                    });
                })
                .AddMemoryStreams("AISmart")
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryGrainStorageAsDefault();
        }
    }
    
    private class TestClientBuilderConfigurator : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder) => clientBuilder
            .AddMemoryStreams("AISmart");
    }
}