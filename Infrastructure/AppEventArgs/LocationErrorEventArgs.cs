using System;

namespace Infrastructure.AppEventArgs
{
    public class LocationErrorEventArgs : EventArgs
    {
        public LocationErrorEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; }
    }
}
