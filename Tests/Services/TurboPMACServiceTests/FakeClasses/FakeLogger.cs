using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurboPMACServiceTests.FakeClasses
{
    public class FakeLogger : ILoggerFacade
    {
        public void Log(string message, Category category, Priority priority)
        {
            MessageReceived?.Invoke(message, category, priority);
        }

        public event Action<string, Category, Priority> MessageReceived;
    }
}
