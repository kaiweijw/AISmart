using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace AISmart.Data;

/* This is used if database provider does't define
 * IAISmartDbSchemaMigrator implementation.
 */
public class NullAISmartDbSchemaMigrator : IAISmartDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
