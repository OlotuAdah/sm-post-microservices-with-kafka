using System.Text.Json;
using System.Text.Json.Serialization;
using CQRS.Core.Events;
using Post.Common.Events;

namespace Post.Query.Infrastructure.Converters;
public class EventJsonConverter : JsonConverter<BaseEvent>
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsAssignableFrom(typeof(BaseEvent));
        // Check if the type is a BaseEvent or it's Subclass e.g PostCreadedEvent
    }

    public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (!JsonDocument.TryParseValue(ref reader, out var doc)) // check if doc is valid json
        {
            throw new JsonException($"Failed to parse {nameof(JsonDocument)}");
        }
        if (!doc.RootElement.TryGetProperty("Type", out var type)) // check if the json has a Type property
        {
            throw new JsonException("Could not detect the Type discriminator property");
        }

        var typeDiscriminator = type.GetString();
        var json = doc.RootElement.GetRawText();
        return typeDiscriminator switch
        {
            // Deserialize the json to the correct type based on the Type property
            nameof(PostCreatedEvent) => JsonSerializer.Deserialize<PostCreatedEvent>(json, options),
            nameof(MessageUpdatedEvent) => JsonSerializer.Deserialize<MessageUpdatedEvent>(json, options),
            nameof(PostLikedEvent) => JsonSerializer.Deserialize<PostLikedEvent>(json, options),
            nameof(PostRemovedEvent) => JsonSerializer.Deserialize<PostRemovedEvent>(json, options),
            nameof(CommentAddedEvent) => JsonSerializer.Deserialize<CommentAddedEvent>(json, options),
            nameof(CommentRemovedEvent) => JsonSerializer.Deserialize<CommentRemovedEvent>(json, options),
            _ => throw new JsonException($"{typeDiscriminator} is not supported by the {nameof(EventJsonConverter)} yet")
        };

    }

    public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
    {
        //not necessary for this project yet, but you can implement it if you want to serialize the event back to json
        throw new NotImplementedException();
    }
}

