using Prism.Logging;

namespace Infrastructure.Interfaces.Logger
{
    public interface ILogMessage
    {
        string Message { get; set; }
        Category MessageCategory { get; set; }
        Priority MessagePriority { get; set; }
    }
}
