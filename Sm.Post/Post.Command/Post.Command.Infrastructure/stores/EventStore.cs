using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.producers;
using Post.Command.Domain.Aggregates;

namespace Post.Command.Infrastructure.stores;
public class EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer) : IEventStore
{
    private readonly IEventStoreRepository _eventStoreRepo = eventStoreRepository;
    private readonly IEventProducer _eventProducer = eventProducer;

    public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
    {
        var eventStream = await _eventStoreRepo.FindByAggregateIdAsync(aggregateId);
        if (eventStream == null || !eventStream.Any())
            throw new AggregateNotFoundException("Incorrect postId provided for aggregate: " + aggregateId);
        return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
    }

    public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
    {
        // TODO: Make the logic atomic, such that _eventStoreRepo.SaveAsync() and _eventProducer.ProduceAsync() are in a single transaction.
        // NB: _eventStoreRepo.SaveAsync() uses mongo db.
        var eventStream = await _eventStoreRepo.FindByAggregateIdAsync(aggregateId);
        if (expectedVersion != -1 && eventStream.Last().Version != expectedVersion)
        {
            throw new ConcurrencyException("Invalid event version");
        }
        var version = expectedVersion;
        foreach (var @event in events)
        {
            version++;
            @event.Version = version;
            var eventType = @event.GetType().Name;

            var eventModel = new EventModel
            {
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = aggregateId,
                AggregateType = nameof(PostAggregate),
                Version = version,
                EventType = eventType,
                EventData = @event,
            };
            await _eventStoreRepo.SaveAsync(eventModel);
            //Once event is saved to the event store, we can publish it to the kafka message broker.
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            await _eventProducer.ProduceAsync(topic, @event);
        }

    }
}
