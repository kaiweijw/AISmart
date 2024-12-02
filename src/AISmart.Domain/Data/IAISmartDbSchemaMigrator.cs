using System.Threading.Tasks;

namespace AISmart.Data;

public interface IAISmartDbSchemaMigrator
{
    Task MigrateAsync();
}
