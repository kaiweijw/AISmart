using System;
using AISmart.Application.Grains;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Orleans.Hosting;
using Orleans.TestingHost;
using Volo.Abp.AutoMapper;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
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
                    services.AddAutoMapper(typeof(AIApplicationGrainsModule).Assembly);
                    var mock = new Mock<ILocalEventBus>();
                    services.AddSingleton(typeof(ILocalEventBus), mock.Object);
                    // Configure logging
                    services.AddLogging(logging =>
                    {
                        logging.AddConsole(); // Adds console logger
                        // Add further logging configurations or providers if needed
                    });
                    services.OnExposing(onServiceExposingContext =>
                    {
                        //Register types for IObjectMapper<TSource, TDestination> if implements
                        var implementedTypes = ReflectionHelper.GetImplementedGenericTypes(
                            onServiceExposingContext.ImplementationType,
                            typeof(IObjectMapper<,>)
                        );
                    });
                    services.AddTransient(
                        typeof(IObjectMapper<>),
                        typeof(DefaultObjectMapper<>)
                    );
                    services.AddTransient(
                        typeof(IObjectMapper),
                        typeof(DefaultObjectMapper)
                    );
                    services.AddTransient(typeof(IAutoObjectMappingProvider),
                        typeof(AutoMapperAutoObjectMappingProvider));
                    services.AddTransient(sp => new MapperAccessor()
                    {
                        Mapper = sp.GetRequiredService<IMapper>()
                    });
                    services.AddTransient<IMapperAccessor>(provider => provider.GetRequiredService<MapperAccessor>());
                    // foreach (var type in implementedTypes)
                    // {
                    // onServiceExposingContext.ExposedTypes.Add(new ServiceIdentifier(type));
                    // }
                })
                .AddMemoryStreams("AISmart")
                .AddMemoryGrainStorage("PubSubStore")
                .AddMemoryGrainStorageAsDefault().AddLogStorageBasedLogConsistencyProvider("LogStorage");
        }
    }
    
    public class MapperAccessor : IMapperAccessor
    {
        public IMapper Mapper { get; set; }
    }
    
    private class TestClientBuilderConfigurator : IClientBuilderConfigurator
    {
        public void Configure(IConfiguration configuration, IClientBuilder clientBuilder) => clientBuilder
            .AddMemoryStreams("AISmart");
    }
}