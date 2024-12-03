using Microsoft.Extensions.DependencyInjection;
using AISmart.Application.Grains;
using AISmart.Domain.Grains;
using AISmart.MongoDB;
using AISmart.Worker.Author;
using AISmart.Worker.Dapr;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Autofac;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Modularity;

namespace AISmart.Worker;

[DependsOn(
    typeof(AbpBackgroundWorkersModule),
    typeof(AbpAspNetCoreMvcModule),
    typeof(AISmartApplicationModule),
    typeof(AISmartApplicationContractsModule),
    typeof(AISmartMongoDbModule),
    typeof(AbpAutofacModule)
)]
public class AISmartWorkerModule : AbpModule, IDomainGrainsModule, IApplicationGrainsModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        context.Services.AddHttpClient();
        context.Services.AddDaprClient();
    }

    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        // add your background workers here
        //await context.AddBackgroundWorkerAsync<AuthorSummaryWorker>();
        await context.AddBackgroundWorkerAsync<DaprTestWorker>();
    }
}