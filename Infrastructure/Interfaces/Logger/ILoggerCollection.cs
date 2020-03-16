using Infrastructure.AppEventArgs;
using System;
using System.Collections.Generic;

namespace Infrastructure.Interfaces.Logger
{
    public interface ILoggerCollection
    {
        List<ILogMessage> Messages { get; }
        event EventHandler<LoggerCollectionEventArgs> NewMessage;
    }
}
