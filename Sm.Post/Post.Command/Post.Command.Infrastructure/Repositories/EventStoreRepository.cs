using CQRS.Core.Domain;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Post.Command.Infrastructure.config;

namespace Post.Command.Infrastructure.Repositories;

public class EventStoreRepository : IEventStoreRepository
{
    private readonly MongoDbConfig _config;
    private readonly IMongoCollection<EventModel> _eventStoreCollection;
    public EventStoreRepository(IOptions<MongoDbConfig> config)
    {
        _config = config.Value;
        var mongoClient = new MongoClient(_config.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(_config.DatabaseName);
        _eventStoreCollection = mongoDatabase.GetCollection<EventModel>(_config.Collection);


    }

    public async Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId)
    {
        return await (await _eventStoreCollection.FindAsync(e => e.AggregateIdentifier == aggregateId)).ToListAsync().ConfigureAwait(false);
        //NB: configureAwait(false) improves performance by forcing the continuation to run on the same thread as the calling method.
        //Without it, the continuation will be scheduled on a different thread, which can lead to performance issues and avoiding deadlocks!

    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }
}