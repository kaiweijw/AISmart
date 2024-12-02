using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AISmart.Data;
using AISmart.DbMigrator.Orleans;
using Orleans;
using Serilog;
using Volo.Abp;
using Volo.Abp.Data;

namespace AISmart.DbMigrator;

public class DbMigratorHostedService : IHostedService
{
    private readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IConfiguration _configuration;

    public DbMigratorHostedService(IHostApplicationLifetime hostApplicationLifetime, IConfiguration configuration)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using (var application = await AbpApplicationFactory.CreateAsync<AISmartDbMigratorModule>(options =>
        { 
            options.Services.AddSingleton<IClusterClient, ClusterClientMock>();
            options.Services.ReplaceConfiguration(_configuration);
            options.UseAutofac();
            options.Services.AddLogging(c => c.AddSerilog());
            options.AddDataMigrationEnvironment();
        }))
        {
            await application.InitializeAsync();

            await application
                .ServiceProvider
                .GetRequiredService<AISmartDbMigrationService>()
                .MigrateAsync();

            await application.ShutdownAsync();

            _hostApplicationLifetime.StopApplication();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
