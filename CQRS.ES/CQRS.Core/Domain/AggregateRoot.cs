

using CQRS.Core.Events;

namespace CQRS.Core.Domain;
public abstract class AggregateRoot
{
    protected Guid _id { get; set; }
    private readonly List<BaseEvent> _changes = new(); //List of events that extends BaseEvent.
    public Guid Id
    {
        get => _id;
    }

    public int Version { get; set; } = -1; //For versioning our aggregate. It's zero-based, so 0 means first version. That's why -1 is used as default.
    public IEnumerable<BaseEvent> GetUncommittedChanges()
    {
        return _changes;
    }

    public void MarkChangesAsCommitted()
    {
        _changes.Clear();
    }

    private void ApplyChange(BaseEvent @event, bool isNew)
    {

        var method = this.GetType().GetMethod("Apply", [@event.GetType()]);
        if (method == null)
        {
            throw new ArgumentNullException($"The Apply method was not found in the aggregate for {@event.GetType().Name}");
        }

        method.Invoke(this, [@event]);
        if (isNew)
        {
            _changes.Add(@event);
        }
    }

    protected void RaiseEvent(BaseEvent @event)
    {
        @event.Version = Version;
        ApplyChange(@event, true); // Apply the event to the aggregate and mark it as new.
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        foreach (var @event in events)
        {
            ApplyChange(@event, false); // not a new event.
        }
        // Set the aggregate version to the last event's version
        if (events?.Any() == true)
        {
            Version = events.Max(e => e.Version);
        }
    }

}


