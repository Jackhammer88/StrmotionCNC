using Prism.Events;

namespace Infrastructure.AggregatorEvents
{
    /// <summary>
    /// Событие сообщающее изменение состояния подключения
    /// </summary>
    public class ConnectionEvent : PubSubEvent<bool> { }
}
