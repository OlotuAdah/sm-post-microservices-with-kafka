using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.producers;
using Polly;

namespace Post.Command.Infrastructure.producers;
public class CircuitBreakerEventProducer : IEventProducer
{
    private readonly IEventProducer _innerProducer;

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var circuitBreaker = Policy
            .Handle<MessageNotPersistedException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30)
            );

        await circuitBreaker.ExecuteAsync(async () =>
            await _innerProducer.ProduceAsync(topic, @event));
    }
}




// Should match your Kafka topic configuration
// public const int REPLICATION_FACTOR = 3;
// public const int MIN_IN_SYNC_REPLICAS = 2;
