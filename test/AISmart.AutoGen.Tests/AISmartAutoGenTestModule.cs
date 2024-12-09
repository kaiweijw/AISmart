using Volo.Abp.Modularity;

namespace AISmart.AutoGen.Tests;

[DependsOn(
    typeof(AISmartTestBaseModule)
)]
public class AISmartAutoGenTestModule: AbpModule
{
    
}