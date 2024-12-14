using System.Threading;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace AISmart;

using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 假设 TLogView 和 TLogEntry 是已定义的类
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
            var newState = ApplyUpdatesToState(new TLogView(), updates);
            var newVersion = currentVersion + 1;

            var newStateWrapper = new MongoStateWrapper<TLogView>
            {
                Version = newVersion,
                State = newState
            };

            await _stateCollection.InsertOneAsync(newStateWrapper);

            // 将事件存储到事件集合中（可选）
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
        // 在这里实现将事件应用于状态的逻辑
        // 取决于你的具体业务逻辑

        return state; // 返回更新后的状态
    }
}

// MongoDB 文档包装器类
public class MongoStateWrapper<T>
{
    public int Version { get; set; }
    public T State { get; set; }
}

public class MongoEventWrapper<T>
{
    public int Version { get; set; }
    public T Event { get; set; }
}