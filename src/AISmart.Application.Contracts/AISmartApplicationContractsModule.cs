using Volo.Abp.Account;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.ObjectExtending;
using Volo.Abp.PermissionManagement;

namespace AISmart;

[DependsOn(
    typeof(AISmartDomainSharedModule),
    typeof(AbpAccountApplicationContractsModule),
    typeof(AbpIdentityApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationContractsModule),
    typeof(AbpObjectExtendingModule)
)]
public class AISmartApplicationContractsModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AISmartDtoExtensions.Configure();
    }
}
