using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Interfaces.Logger
{
    public interface ILoggerExtended
    {
        void Fatal(string message, Exception ex = null);
        void Warn(string message, Exception ex = null);
        void Exception(string message, Exception ex = null);
        void Info(string message);
    }
}
