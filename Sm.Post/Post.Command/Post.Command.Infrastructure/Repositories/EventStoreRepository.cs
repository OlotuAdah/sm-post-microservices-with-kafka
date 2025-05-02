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

    public async Task<List<EventModel>> FindAllEventsAsync()
    {
        return await _eventStoreCollection.Find(_ => true).ToListAsync().ConfigureAwait(false);
    }

    public async Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId)
    {
        var filter = Builders<EventModel>.Filter.Eq(e => e.AggregateIdentifier, aggregateId);
        var eventStoreCollection = await _eventStoreCollection.FindAsync(filter);
        var events = await eventStoreCollection.ToListAsync().ConfigureAwait(false);
        return events;


    }

    public async Task SaveAsync(EventModel @event)
    {
        await _eventStoreCollection.InsertOneAsync(@event).ConfigureAwait(false);
    }
}