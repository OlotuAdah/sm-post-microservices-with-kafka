using Amazon.Runtime.Internal.Util;
using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Handlers;
using CQRS.Core.Infrastructure;
using CQRS.Core.producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Command.Domain.Aggregates;
using Post.Common.Settings;

namespace Post.Command.Infrastructure.Handlers;
public class EventSourcingHandler(IEventStore eventStore, IEventProducer eventProducer, IOptions<KafkaTopics> kafkaTopics, ILogger<EventSourcingHandler> logger) : IEventSourcingHandler<PostAggregate>
{
    private readonly IEventStore _eventStore = eventStore;
    private readonly IEventProducer _eventProducer = eventProducer;
    private readonly ILogger<EventSourcingHandler> _logger = logger;
    private readonly KafkaTopics _kafkaTopics = kafkaTopics.Value;

    public async Task<PostAggregate> GetByIdAsync(Guid aggregateId)
    {
        var aggregate = new PostAggregate();
        var events = await _eventStore.GetEventsAsync(aggregateId);
        if (events == null || !events.Any()) return aggregate;
        aggregate.ReplayEvents(events);
        aggregate.Version = events.Select(x => x.Version).Max();
        return aggregate;
    }

    public async Task RepublishEventsAsync()
    {
        List<BaseEvent>? allEvents = [];
        List<Guid> aggregateIds = await _eventStore.GetAggregateIdsAsync();
        if (aggregateIds == null || aggregateIds.Count == 0)
        {
            _logger.LogInformation("No aggregate ids found in the event store.");
            return;
        }
        IEnumerable<Task>? tasks = aggregateIds.Select(async aggregateId =>
        {
            PostAggregate aggregate = await GetByIdAsync(aggregateId);
            if (aggregate == null || !aggregate.Active)
            {
                _logger.LogInformation($"Aggregate with id {aggregateId} not found or not active.");
                return;
            }
            List<BaseEvent>? events = await _eventStore.GetEventsAsync(aggregateId);
            if (events == null || events.Count == 0) return;
            allEvents.AddRange(events);
        });
        await Task.WhenAll(tasks);

        //Remove duplicates from the list of events.
        // allEvents = [.. allEvents.GroupBy(x => x.Version).Select(x => x.First())];
        //Publish events to the event producer. 
        if (allEvents == null || allEvents.Count == 0)
        {
            _logger.LogInformation("No events to republish to read DB.");
            return;
        }
        allEvents = [.. allEvents.OrderBy(x => x.Version)];
        foreach (BaseEvent @event in allEvents)
        {
            await _eventProducer.ProduceAsync(_kafkaTopics.SmPostTopic, @event);
        }
    }

    public async Task SaveAsync(AggregateRoot aggregateRoot)
    {
        await _eventStore.SaveEventsAsync(aggregateRoot.Id, aggregateRoot.GetUncommittedChanges(), aggregateRoot.Version);
        aggregateRoot.MarkChangesAsCommitted();
    }
}

