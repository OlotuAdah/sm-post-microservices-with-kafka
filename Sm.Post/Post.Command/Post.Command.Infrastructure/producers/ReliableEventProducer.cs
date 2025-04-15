using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.producers;
using Microsoft.Extensions.Logging;

namespace Post.Command.Infrastructure.producers;
public class ReliableEventProducer(ProducerConfig producerConfig, ILogger<ReliableEventProducer> logger) : IEventProducer
{
    private readonly ProducerConfig _producerConfig = producerConfig;
    private readonly ILogger<ReliableEventProducer> _logger = logger;

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();

        var message = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };

        try
        {
            // Produce the message and handle the delivery report
            var result = await producer.ProduceAsync(topic, message);

            if (result.Status != PersistenceStatus.Persisted)
            {
                _logger.LogWarning(
                    "Message {MessageId} not properly persisted. Status: {Status}",
                    result.Message.Key, result.Status);

                throw new MessageNotPersistedException(
                    $"Message not properly persisted. Status: {result.Status}");
            }

            _logger.LogInformation(
                "Message {MessageId} successfully persisted to topic {Topic}",
                result.Message.Key, topic);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to produce message to topic {Topic}", topic);
            throw;
        }
    }
}
