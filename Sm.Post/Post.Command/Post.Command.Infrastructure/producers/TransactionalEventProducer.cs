using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.producers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Post.Command.Infrastructure.producers;
public class TransactionalEventProducer(IOptions<ProducerConfig> config, ILogger<TransactionalEventProducer> logger) : IEventProducer
{
    private readonly ProducerConfig _config = config.Value;
    private readonly ILogger<TransactionalEventProducer> _logger = logger;

    public Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent
    {
        throw new NotImplementedException();
    }

    public async Task ProduceInTransactionAsync<T>(
        string topic,
        IEnumerable<T> events) where T : BaseEvent
    {
        _config.TransactionalId = "tx-" + Guid.NewGuid().ToString();

        using var producer = new ProducerBuilder<string, string>(_config).Build();
        producer.InitTransactions(TimeSpan.FromSeconds(10));

        try
        {
            producer.BeginTransaction();

            foreach (var @event in events)
            {
                var result = await producer.ProduceAsync(topic,
                    new Message<string, string>
                    {
                        Key = Guid.NewGuid().ToString(),
                        Value = JsonSerializer.Serialize(@event, @event.GetType())
                    });

                if (result.Status != PersistenceStatus.Persisted)
                {
                    producer.AbortTransaction();
                    throw new MessageNotPersistedException(
                        $"Message not persisted. Status: {result.Status}");
                }
            }

            producer.CommitTransaction();
        }
        catch (Exception ex)
        {
            // Log the exception (optional)
            _logger.LogError($"Transaction failed: {ex.Message}");
            // Abort the transaction if any error occurs 
            producer.AbortTransaction();
            throw;
        }
    }
}
