using System.Text.Json;
using Confluent.Kafka;
using CQRS.Core.Consumers;
using CQRS.Core.Events;
using Microsoft.Extensions.Options;
using Post.Query.Infrastructure.Converters;
using Post.Query.Infrastructure.Handlers;

namespace Post.Query.Infrastructure.Consumers;
public class EventConsumer(IOptions<ConsumerConfig> consumerConfig, IEventHandler eventHandler) : IEventConsumer
{
    private readonly ConsumerConfig consumerConfig = consumerConfig.Value;
    private readonly IEventHandler _eventHandler = eventHandler;


    public void Consume(string topic)
    {
        var consumerBuilder = new ConsumerBuilder<string, string>(consumerConfig)
        .SetKeyDeserializer(Deserializers.Utf8)
        .SetValueDeserializer(Deserializers.Utf8);


        var consumer = consumerBuilder.Build();
        consumer.Subscribe(topic);

        while (true)
        {
            var consumerResult = consumer.Consume();
            if (consumerResult?.Message?.Value == null) continue;
            var customJsonConverterOptions = new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() }
            };
            // Deserialize the message value to the base event type using the custom json converter options 
            // to handle the polymorphic deserialization of the event types based on the Type property in the json
            // and then call the event handler to handle the event and pass the deserialized event to it
            var @event = JsonSerializer.Deserialize<BaseEvent>(consumerResult.Message.Value, customJsonConverterOptions);
            var handlerMethod = _eventHandler.GetType().GetMethod("On", [@event.GetType()]);
            if (handlerMethod == null) throw new ArgumentNullException(nameof(handlerMethod), $"Handler method not found for event type {@event.GetType()}");
            // Invoke the handler method with the event as a parameter
            handlerMethod?.Invoke(_eventHandler, [@event]);
            // handlerMethod?.Invoke(_eventHandler, new object [] { @event }); // this is the same as above but with an array of objects as a parameter
            // commit the message after processing it. Tell kafka that we have processed the message and it can be removed from the topic
            consumer.Commit(consumerResult); // this will increament kafka's commits log offset for this consumer group and topic

        }
    }
}
