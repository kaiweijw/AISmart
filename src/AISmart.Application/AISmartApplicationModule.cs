using System;
using AISmart.Application;
using AISmart.Application.Grains;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using Volo.Abp.Account;
using Volo.Abp.AspNetCore.Mvc.Dapr;
using Volo.Abp.AutoMapper;
using Volo.Abp.Dapr;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace AISmart;

[DependsOn(
    typeof(AISmartDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(AISmartApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpDaprModule),
    typeof(AbpAspNetCoreMvcDaprModule),
    typeof(AIApplicationGrainsModule),
    typeof(AISmartGAgentAElfModule),
    typeof(AISmartGAgentTelegramModule),
    typeof(AISmartGAgentTwitterModule)
)]
public class AISmartApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AISmartApplicationModule>();
        });
        /*context.Services.AddMediatR(typeof(CreateTransactionCommandHandler).Assembly);
        context.Services.AddTransient<CreateTransactionCommandHandler>();
        context.Services.AddSingleton<IElasticClient>(provider =>
        {
            var settings =new ConnectionSettings(new Uri("http://127.0.0.1:9200"))
                .DefaultIndex("cqrs");
            return new ElasticClient(settings);
        });*/
    }
}
