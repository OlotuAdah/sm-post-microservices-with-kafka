
using CQRS.Core.Events;

namespace CQRS.Core.Domain;
public interface IEventStoreRepository
{
    Task SaveAsync(EventModel @eventModel);
    Task<List<EventModel>> FindByAggregateIdAsync(Guid aggregateId);

    // NB: Events are immutable, so we don't need to update them. 

}
