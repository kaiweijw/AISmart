using Volo.Abp.Modularity;

namespace AISmart;

[DependsOn(
    typeof(AISmartApplicationModule),
    typeof(AISmartDomainTestModule)
)]
public class AISmartApplicationTestModule : AbpModule
{

}
