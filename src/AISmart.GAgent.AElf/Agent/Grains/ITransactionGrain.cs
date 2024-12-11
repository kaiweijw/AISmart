using System.Threading.Tasks;
using AISmart.Agent.Event;
using AISmart.Dto;
using Orleans;

namespace AISmart.Agent.Grains;

public interface ITransactionGrain:IGrainWithGuidKey
{
    Task <TransactionDto> SendAElfTransactionAsync(SendTransactionSEvent sendTransactionSEvent);
    Task <TransactionDto> LoadAElfTransactionResultAsync(QueryTransactionSEvent queryTransactionSEvent);
    
    Task <TransactionDto> GetAElfTransactionAsync(QueryTransactionSEvent queryTransactionSEvent);
}