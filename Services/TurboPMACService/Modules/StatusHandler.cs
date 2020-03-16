using Infrastructure.Enums;
using Infrastructure.SharedClasses;
using System;
using System.Collections.Generic;

namespace ControllerService.Modules
{
    public class StatusHandler : IObservable<MachineStatuses>
    {
        private List<IObserver<MachineStatuses>> _observers;

        public StatusHandler()
        {
            _observers = new List<IObserver<MachineStatuses>>();
        }
        public IDisposable Subscribe(IObserver<MachineStatuses> observer)
        {
            if (!_observers.Contains(observer))
                _observers.Add(observer);
            return new Unsubscriber<MachineStatuses>(_observers, observer);
        }

        public void StatusesUpdated(GlobalStatusXs xs, GlobalStatusYs ys, IEnumerable<Tuple<MotorXStatuses, MotorYStatuses>> motors)
        {
            var updated = new MachineStatuses(xs, ys, motors);
            foreach (var observer in _observers)
            {
                observer.OnNext(updated);
            }
        }

        private class Unsubscriber<MachineStatuses> : IDisposable
        {
            private List<IObserver<MachineStatuses>> _observers;
            private IObserver<MachineStatuses> _observer;

            internal Unsubscriber(List<IObserver<MachineStatuses>> observers, IObserver<MachineStatuses> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}