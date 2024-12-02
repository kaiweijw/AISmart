using Volo.Abp.Modularity;

namespace AISmart;

/* Inherit from this class for your domain layer tests. */
public abstract class AISmartDomainTestBase<TStartupModule> : AISmartTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
