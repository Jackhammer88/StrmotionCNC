using Infrastructure.AppEventArgs;
using Infrastructure.Interfaces.Logger;
using NLog;
using Prism.Logging;
using Prism.Modularity;
using System;
using System.Collections.Generic;

namespace LoggerService
{

    [Module(ModuleName = "LoggerModule")]
    public class LoggerExtended : ILoggerFacade, ILoggerCollection, ILoggerExtended
    {
        public List<ILogMessage> Messages { get; } = new List<ILogMessage>();
        private const int _maxCollectionSize = 500;

        public LoggerExtended()
        {
            Logger = LogManager.GetCurrentClassLogger();
            WriteLoggerInitialMessage();
        }

        private void WriteLoggerInitialMessage()
        {
            var lTime = $"---------------------------------------------Launching time: {DateTime.Now}------------------------------------------";
            Logger.Info(lTime);
            Logger.Error(lTime);
        }

        public void Log(string message, Category category, Priority priority)
        {
            var messageText = $"{DateTime.Now}: {message}\r\n";
            var logMessage = new LogMessage { Message = messageText, MessageCategory = category, MessagePriority = priority };

            if (category != Category.Exception)
            {
                Messages.Add(logMessage);
                NewMessage(this, new LoggerCollectionEventArgs(logMessage));
            }

            switch (category)
            {
                case Category.Debug:
                    Logger.Debug(message);
                    break;
                case Category.Exception:
                    Logger.Fatal(message);
                    break;
                case Category.Info:
                    Logger.Info(message);
                    break;
                case Category.Warn:
                    Logger.Warn(message);
                    break;
                default:
                    break;
            }

            TrimCollectionSize();
        }
        public void Info(string message)
        {
            WriteToLog(Logger.Info, message);
            Log(message, Category.Info, Priority.High);
        }
        public void Fatal(string message, Exception ex = null)
        {
            if (ex != null)
                WriteToLog(Logger.Fatal, message, ex);
            else
                WriteToLog(Logger.Fatal, message);
            Log(message, Category.Exception, Priority.High);
        }
        public void Warn(string message, Exception ex = null)
        {
            if (ex != null)
                WriteToLog(Logger.Warn, message, ex);
            else
                WriteToLog(Logger.Warn, message);
            Log(message, Category.Warn, Priority.High);
        }
        public void Exception(string message, Exception ex = null)
        {
            if (ex != null)
                WriteToLog(Logger.Fatal, message, ex);
            else
                WriteToLog(Logger.Fatal, message);
            Log(message, Category.Exception, Priority.High);
        }
        private void TrimCollectionSize()
        {
            if (Messages.Count >= _maxCollectionSize)
            {
                Messages.RemoveRange(0, _maxCollectionSize / 5 * 4);
            }
        }
        private void WriteToLog(Action<string> action, string message)
        {
            action(message);
        }
        private void WriteToLog(Action<Exception, string> action, string message, Exception ex)
        {
            action(ex, message);
        }

        private Logger Logger { get; }
        public event EventHandler<LoggerCollectionEventArgs> NewMessage = delegate { };
    }
}
