using Infrastructure.ApplicationStates;
using System;

namespace Infrastructure.AppEventArgs
{
    public class ProgramStateEventArgs : EventArgs
    {
        public ProgramStateEventArgs(ProgramState programState)
        {
            State = programState;
        }
        public ProgramState State { get; }
    }
}
