using System.Threading;
using Google.Cloud.AIPlatform.V1;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace AISmart;

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MongoStorage<TLogView, TLogEntry> : JournaledGrain<TLogView, TLogEntry>, ICustomStorageInterface<TLogView,TLogEntry>,IGrainWithIntegerKey where TLogView : class, new() where TLogEntry : class
{
    private readonly IMongoCollection<MongoStateWrapper<TLogView>> _stateCollection;
    private readonly IMongoCollection<MongoEventWrapper<TLogEntry>> _eventCollection;

    // public MongoStorage(string connectionString, string databaseName, string stateCollectionName, string eventCollectionName)
    // {
    //     var client = new MongoClient(connectionString);
    //     var database = client.GetDatabase(databaseName);
    //
    //     _stateCollection = database.GetCollection<MongoStateWrapper<TLogView>>(stateCollectionName);
    //     _eventCollection = database.GetCollection<MongoEventWrapper<TLogEntry>>(eventCollectionName);
    // }
    
    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask; // do not wait for initial load
    }
    
    public MongoStorage()
    {
        string connectionString = "mongodb://localhost:27017";
        string databaseName = "AISmart";
        string stateCollectionName = "state";
        string eventCollectionName = "event";
        
        var client = new MongoClient(connectionString);
        var database = client.GetDatabase(databaseName);
    
        _stateCollection = database.GetCollection<MongoStateWrapper<TLogView>>(stateCollectionName);
        _eventCollection = database.GetCollection<MongoEventWrapper<TLogEntry>>(eventCollectionName);
    }

    public async Task<KeyValuePair<int, TLogView>> ReadStateFromStorage()
    {
        var stateWrapper = await _stateCollection.Find(FilterDefinition<MongoStateWrapper<TLogView>>.Empty)
                                                 .SortByDescending(s => s.Version)
                                                 .FirstOrDefaultAsync();

        if (stateWrapper != null)
        {
            return new KeyValuePair<int, TLogView>(stateWrapper.Version, stateWrapper.State);
        }
        
        return new KeyValuePair<int, TLogView>(0, default(TLogView));
    }

    public async Task<bool> ApplyUpdatesToStorage(IReadOnlyList<TLogEntry> updates, int expectedVersion)
    {
        var stateWrapper = await _stateCollection.Find(FilterDefinition<MongoStateWrapper<TLogView>>.Empty)
                                                 .SortByDescending(s => s.Version)
                                                 .FirstOrDefaultAsync();

        int currentVersion = stateWrapper?.Version ?? 0;

        if (expectedVersion != currentVersion)
        {
            return false;
        }

        try
        {
            var newState = ApplyUpdatesToState(stateWrapper.State, updates);
            var newVersion = currentVersion + 1;

            var newStateWrapper = new MongoStateWrapper<TLogView>
            {
                Version = newVersion,
                State = newState
            };

            await _stateCollection.InsertOneAsync(newStateWrapper);

            foreach (var update in updates)
            {
                var eventWrapper = new MongoEventWrapper<TLogEntry>
                {
                    Version = newVersion,
                    Event = update
                };
                await _eventCollection.InsertOneAsync(eventWrapper);
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private TLogView ApplyUpdatesToState(TLogView state, IReadOnlyList<TLogEntry> updates)
    {
        var apply = typeof(TLogView).GetMethod("Apply", new[] { updates[0].GetType() });
        foreach (var updateEntry in updates)
        {
            apply.Invoke(State, new object[] { updateEntry });
        }
        return State; 
    }

}

public class MongoStateWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public int Version { get; set; }
    public T State { get; set; }
}

public class MongoEventWrapper<T>
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public int Version { get; set; }
    public T Event { get; set; }
}