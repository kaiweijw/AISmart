using System.Threading.Tasks;
using AISmart.CQRS.Dto;

namespace AISmart.CQRS;

public interface IElasticIndexService
{
    public void CheckExistOrCreateIndex(string typeName);
    public Task SaveOrUpdateIndexAsync(string typeName,BaseStateIndex baseStateIndex);

}