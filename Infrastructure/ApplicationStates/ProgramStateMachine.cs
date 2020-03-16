using Infrastructure.AppEventArgs;
using System;

namespace Infrastructure.ApplicationStates
{
    public abstract class ProgramStateMachine
    {
        private ProgramState _currentState;
        public ProgramState CurrentState
        {
            get => _currentState;
            set
            {
                if (_currentState != value)
                    _currentState = value;
                OnStateChanged(this, new ProgramStateEventArgs(_currentState));
            }
        }

        public event EventHandler<ProgramStateEventArgs> OnStateChanged = delegate { };
    }
}
