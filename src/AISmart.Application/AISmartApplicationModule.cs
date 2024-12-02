using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;

namespace AISmart;

[DependsOn(
    typeof(AISmartDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(AISmartApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule)
    )]
public class AISmartApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AISmartApplicationModule>();
        });
    }
}
