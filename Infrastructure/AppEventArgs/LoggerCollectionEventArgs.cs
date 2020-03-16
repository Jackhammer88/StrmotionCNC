using Infrastructure.Interfaces.Logger;
using System;

namespace Infrastructure.AppEventArgs
{
    public class LoggerCollectionEventArgs : EventArgs
    {
        public LoggerCollectionEventArgs(ILogMessage message)
        {
            Message = message;
        }
        public ILogMessage Message { get; }
    }
}