using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.VirtualFileSystem;

namespace AISmart;

[DependsOn(
    typeof(AISmartApplicationContractsModule),
    typeof(AbpAccountHttpApiClientModule),
    typeof(AbpIdentityHttpApiClientModule),
    typeof(AbpPermissionManagementHttpApiClientModule)
)]
public class AISmartHttpApiClientModule : AbpModule
{
    public const string RemoteServiceName = "Default";

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.AddHttpClientProxies(
            typeof(AISmartApplicationContractsModule).Assembly,
            RemoteServiceName
        );

        Configure<AbpVirtualFileSystemOptions>(options =>
        {
            options.FileSets.AddEmbedded<AISmartHttpApiClientModule>();
        });
    }
}
