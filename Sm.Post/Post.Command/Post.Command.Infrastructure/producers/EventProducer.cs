using System.Text.Json;
using Amazon.Runtime.Internal.Util;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;

namespace Post.Command.Infrastructure.producers;
public class EventProducer(IOptions<ProducerConfig> producerConfig, ILogger<EventProducer> logger) : IEventProducer
{
    private readonly ProducerConfig _producerConfig = producerConfig.Value;
    private readonly ILogger<EventProducer> _logger = logger;

    public async Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        using var producer = new ProducerBuilder<string, string>(_producerConfig)
            .SetKeySerializer(Serializers.Utf8)
            .SetValueSerializer(Serializers.Utf8)
            .Build();
        //Message must have the same vaue type as the producer value type
        //In this case, the producer value type is string, so the message value must also be string
        var eventMessage = new Message<string, string>
        {
            Key = Guid.NewGuid().ToString(),
            Value = JsonSerializer.Serialize(@event, @event.GetType())
        };
        _logger.LogInformation("Producing message to topic {topic} with value {value}", topic, eventMessage.Value);
        // ProduceAsync is an async method that sends the message to the topic and returns a DeliveryResult object
        var deliveryResult = await producer.ProduceAsync(topic, eventMessage);
        if (deliveryResult.Status != PersistenceStatus.Persisted)
        {
            throw new Exception($"Could not produce ${@event.GetType().Name} message to topic ${topic} due to the following error: '{deliveryResult.Message.Value}' ");
        }
        _logger.LogInformation($"Message {deliveryResult.Message.Value} sent to topic {topic}");
    }

    public async Task ProduceWithRetryAsync<T>(string topic, T @event) where T : BaseEvent
    {
        DeliveryResult<string, string>? deliveryResult = null;
        var retryPolicy = Policy
            .Handle<ProduceException<string, string>>()
            .WaitAndRetryAsync(3, retryAttempt =>
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        await retryPolicy.ExecuteAsync(async () =>
        {
            using var producer = new ProducerBuilder<string, string>(_producerConfig)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();

            deliveryResult = await producer.ProduceAsync(topic, new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event, @event.GetType())
            });
        });
        if (deliveryResult?.Status != PersistenceStatus.Persisted)
        {
            throw new Exception($"Could not produce {@event.GetType().Name} message to topic {topic} due to the following error: '{deliveryResult?.Message.Value}' ");
        }
        Console.WriteLine($"Message {deliveryResult?.Message.Value} sent to topic {topic}");
    }

}


// From AmazonQ, may grow the config to include these settings later.
//   ProducerConfig
//     {
//         BootstrapServers = "localhost:9092",
//         ClientId = "event-producer",
//         // Reliability settings
//         EnableIdempotence = true,
//         Acks = Acks.All,
//         MessageSendMaxRetries = 3,
//         // Performance settings
//         BatchSize = 16384,
//         LingerMs = 1
//     };