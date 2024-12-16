using AElf.Indexing.Elasticsearch;
using Nest;
using Volo.Abp.Domain.Entities;

namespace AISmart.Application.Grains.Dto;

public class CreateTransactionEventIndex:  IEntity<string>,IIndexBuild
{
    [Keyword] public string Id { get; set; }
    [Keyword] public string EventType { get; set; }
    [Keyword] public string EventMessage{ get; set; }

    public object?[] GetKeys()
    {
        throw new NotImplementedException();
    }
}