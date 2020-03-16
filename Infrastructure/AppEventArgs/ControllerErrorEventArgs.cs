using System;

namespace Infrastructure.AppEventArgs
{
    public class ControllerErrorEventArgs : EventArgs
    {
        public ControllerErrorEventArgs(long code, string message)
        {
            ErrorCode = code;
            Message = message;
        }
        public long ErrorCode { get; }
        public string Message { get; }
    }
}