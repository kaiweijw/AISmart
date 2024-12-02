using Volo.Abp.Modularity;

namespace AISmart;

[DependsOn(
    typeof(AISmartDomainModule),
    typeof(AISmartTestBaseModule)
)]
public class AISmartDomainTestModule : AbpModule
{

}
