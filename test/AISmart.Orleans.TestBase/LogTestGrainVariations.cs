using Orleans.Providers;
using Orleans.Serialization;

namespace AISmart.LogConsistencyProvider.Mongo
{
    
    // use the explictly specified "MongoStorage" log-consistency provider with symmetric access from all clusters
    [LogConsistencyProvider(ProviderName = "MongoStorage")]
    public class LogTestGrainCustomStorage : 
        Orleans.EventSourcing.CustomStorage.ICustomStorageInterface<MyGrainState, object>
    {

        // we use another impl of this grain as the primary.
        private ILogTestGrain storagegrain;

        private ILogTestGrain GetStorageGrain()
        {
            if (storagegrain == null)
            {
                storagegrain = GrainFactory.GetGrain<ILogTestGrain>(this.GetPrimaryKeyLong(), "TestGrains.LogTestGrainSharedStateStorage");
            }
            return storagegrain;
        }
 

        public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<object> updates, int expectedversion)
        {
            return GetStorageGrain().Update(updates, expectedversion);
        }

        public async Task<KeyValuePair<int, MyGrainState>> ReadStateFromStorage()
        {
            var kvp = await GetStorageGrain().Read();
            return new KeyValuePair<int, MyGrainState>(kvp.Key, (MyGrainState)kvp.Value);
        }
    }


}
