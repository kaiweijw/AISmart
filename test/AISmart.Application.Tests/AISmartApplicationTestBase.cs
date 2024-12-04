using Volo.Abp.Modularity;
using Xunit.Abstractions;

namespace AISmart;

public abstract class AISmartApplicationTestBase<TStartupModule> : AISmartOrleansTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
}
