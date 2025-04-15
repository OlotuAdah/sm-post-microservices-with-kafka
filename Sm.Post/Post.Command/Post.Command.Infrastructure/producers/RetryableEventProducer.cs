using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.producers;
using Microsoft.Extensions.Logging;
using Polly;

namespace Post.Command.Infrastructure.producers;
public class RetryableEventProducer : IEventProducer
{
    private readonly IEventProducer _innerProducer;
    private readonly ILogger<RetryableEventProducer> _logger;

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        var retryPolicy = Policy
            .Handle<MessageNotPersistedException>()
            .Or<ProduceException<string, string>>()
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    _logger.LogWarning(
                        exception,
                        "Retry {RetryCount} after {Delay}ms",
                        retryCount,
                        timeSpan.TotalMilliseconds);
                }
            );

        try
        {
            await retryPolicy.ExecuteAsync(async () =>
                await _innerProducer.ProduceAsync(topic, @event));
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                $"Failed to produce event ${@event.GetType().Name} to topic {topic} after max retries",
                @event.GetType().Name,
                topic);
            throw;
        }
    }
}
