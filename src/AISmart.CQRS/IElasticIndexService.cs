using System.Threading.Tasks;

namespace AISmart.CQRS;

public interface IElasticIndexService
{
    public Task CreateIndexFromEntity(string typeName);

}