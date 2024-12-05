// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using AISmart.Domain.Grains.Event;
// using Nest;
// using Orleans.EventSourcing;
//
// namespace AISmart.Dapter;
//
// public class ESLogViewAdaptor:ILogViewAdaptor<AgentTaskState, AgentTaskEvent>
// {
//     private readonly IElasticClient _client;
//     private static readonly string _indexKey = "orleans-logs";
//
//     public ESLogViewAdaptor(IElasticClient elasticClient)
//     {
//         _client = elasticClient;
//     }
//     
//     public Task<IReadOnlyList<AgentTaskEvent>> RetrieveLogSegment(int fromVersion, int toVersion)
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public AgentTaskState TentativeView { get; }
//     public AgentTaskState ConfirmedView { get; }
//     public int ConfirmedVersion { get; }
//     public IEnumerable<AgentTaskEvent> UnconfirmedSuffix { get; }
//     
//     public async void Submit(AgentTaskEvent entry)
//     {
//         await _client.IndexDocumentAsync(entry);
//     }
//
//     public async  void SubmitRange(IEnumerable<AgentTaskEvent> entries)
//     {
//         foreach (var entry in entries)
//         {
//             await _client.IndexDocumentAsync(entry);
//         }
//     }
//
//     public async Task<bool> TryAppend(AgentTaskEvent entry)
//     {
//         var existing = await _client.SearchAsync<AgentTaskEvent>(s => s
//             .Index("orleans-logs")
//             .Query(q => q
//                             .Term(t => t.Field(f => f.TaskId).Value(entry.TaskId)) &&
//                         q.Term(t => t.Field(f => f.Id).Value(entry.Id))));
//
//         if (existing.Documents.Any())
//         {
//             return false; 
//         }
//
//         await _client.IndexDocumentAsync(entry);
//         return true;
//     }
//
//     public Task<bool> TryAppendRange(IEnumerable<AgentTaskEvent> entries)
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public async Task ConfirmSubmittedEntries()
//     {
//         await _client.UpdateByQueryAsync<LogEntry>(u => u
//             .Index("orleans-logs")
//             .Query(q => q
//                             .Term(t => t.Field(f => f.GrainId).Value(_grainId)) &&
//                         q.Range(r => r
//                             .Field(f => f.SequenceNumber)
//                             .LessThanOrEquals(upToSequenceNumber)))
//             .Script(s => s
//                 .Source("ctx._source.confirmed = true")));
//     }
//
//     public Task Synchronize()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void EnableStatsCollection()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public void DisableStatsCollection()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public LogConsistencyStatistics GetStats()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public Task PreOnActivate()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public Task PostOnActivate()
//     {
//         throw new System.NotImplementedException();
//     }
//
//     public Task PostOnDeactivate()
//     {
//         throw new System.NotImplementedException();
//     }
// }