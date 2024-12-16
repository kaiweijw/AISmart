/*using AElf.Indexing.Elasticsearch;
using Nest;
using Volo.Abp.Domain.Entities;

namespace AISmart.Application.Grains.Dto;

public class CreateTransactionEventIndex: IIndexBuild, IEntity<string>
{
    [Keyword] public string Id { get; set; }
    [Keyword] public string ChainId { get; set; }
    [Keyword] public string SenderName{ get; set; }
    [Keyword] public  string ContractAddress { get; set; }
    [Keyword] public  string MethodName { get; set; }
    [Keyword] public  string Param { get; set; }
    [Keyword] public bool IsSuccess   { get; set; }
    [Keyword] public string TransactionId { get; set; }

    public object?[] GetKeys()
    {
        throw new NotImplementedException();
    }
}*/