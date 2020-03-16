using System;

namespace Infrastructure.AppEventArgs
{
    public class ControllerLostConnectionEventArgs : EventArgs
    {
        public ControllerLostConnectionEventArgs(long code)
        {
            Code = code;
        }
        public long Code { get; }
    }
}