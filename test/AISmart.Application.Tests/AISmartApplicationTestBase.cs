using Volo.Abp.Modularity;

namespace AISmart;

public abstract class AISmartApplicationTestBase<TStartupModule> : AISmartTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
