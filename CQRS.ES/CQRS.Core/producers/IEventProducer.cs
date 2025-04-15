using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CQRS.Core.Events;

namespace CQRS.Core.producers;
public interface IEventProducer
{
    Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;

}
//where T : BaseEvent ==> All the concrete event classes  should extends BaseEvent
//In other words, T should always be one of our concrete event classes.


