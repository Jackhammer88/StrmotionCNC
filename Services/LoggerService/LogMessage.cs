using Infrastructure.Interfaces.Logger;
using Prism.Logging;

namespace LoggerService
{
    public class LogMessage : ILogMessage
    {
        public string Message { get; set; }
        public Category MessageCategory { get; set; }
        public Priority MessagePriority { get; set; }
    }
}
