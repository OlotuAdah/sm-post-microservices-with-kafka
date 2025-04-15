

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
        //new gets added to list of uncommitted changes
        //method is invoked on the aggregate
        //Version is incremented
        // Version++;
        var isNewEvent = true; // we are raising an even, then it's a new event.
        ApplyChange(@event, isNewEvent);
    }

    public void ReplayEvents(IEnumerable<BaseEvent> events)
    {
        //Doesn't add events to the list of uncommitted changes.
        //Method is invoked on the aggregate.
        //Version is incremented.
        // Version++;
        foreach (var @event in events)
        {
            ApplyChange(@event, false); // not a new event.
        }
    }

}


