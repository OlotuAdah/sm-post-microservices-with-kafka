using CQRS.Core.Consumers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Post.Query.Infrastructure.Consumers;

public class ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider) : IHostedService
{
    private readonly ILogger<ConsumerHostedService> _logger = logger;
    private readonly IServiceProvider _serviceprovider = serviceProvider;
    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Event consumer service is starting.");
        using (IServiceScope scope = _serviceprovider.CreateScope())
        {
            var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            if (string.IsNullOrEmpty(topic)) throw new ArgumentNullException("KAFKA_TOPIC", "Kafka topic not found in environment variables.");
            Task.Run(() => eventConsumer.Consume(topic), cancellationToken); // Start consuming messages from the topic
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
