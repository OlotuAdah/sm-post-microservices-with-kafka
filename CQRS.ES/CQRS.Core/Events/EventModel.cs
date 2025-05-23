
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CQRS.Core.Events;
public class EventModel
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } //MongoDB ID
    public DateTime Timestamp { get; set; }
    public Guid AggregateIdentifier { get; set; } // identifier of the aggregate  that the event relates to.
    public string AggregateType { get; set; }
    public int Version { get; set; }
    public string EventType { get; set; }
    public BaseEvent EventData { get; set; }
}