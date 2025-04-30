using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Post.Common.Settings;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider, IOptions<KafkaTopics> kafkaTopics) : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger = logger;
    private readonly IServiceProvider _serviceprovider = serviceProvider;
    private readonly KafkaTopics _kafkaTopics = kafkaTopics.Value;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service is starting.");
        using (IServiceScope scope = _serviceprovider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            if (string.IsNullOrEmpty(_kafkaTopics.SmPostTopic)) throw new ArgumentNullException("KAFKA_TOPIC", "Kafka topic not found in environment variables.");
            Task.Run(() => eventConsumer.Consume(_kafkaTopics.SmPostTopic), cancellationToken); // Start consuming messages from the topic
        }
        _logger.LogInformation("Event consumer service started.");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service is stopping.");
        using var scope = _serviceprovider.CreateScope();
        _logger.LogInformation("Event consumer service is stopped.");
        // Stop consuming messages and clean up resources here
        return Task.CompletedTask;
    }
}
