using Infrastructure.SharedClasses;
using System;

namespace GeneralComponents.Models
{
    internal class StatusesNotificationClient : IObserver<MachineStatuses>
    {
        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
        }

        public void OnNext(MachineStatuses value)
        {
            OnStatusesUpdated(this, value);
        }

        public event EventHandler<MachineStatuses> OnStatusesUpdated = delegate { };
    }
}