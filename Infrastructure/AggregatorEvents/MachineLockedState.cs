using Prism.Events;

namespace Infrastructure.AggregatorEvents
{
    //Вызывается когда контроллер занят выполнением какой-то задачи
    public class MachineLockedState : PubSubEvent<bool>
    {
    }
}
