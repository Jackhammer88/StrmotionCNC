using Infrastructure.Abstract;
using Infrastructure.Interfaces.Logger;
using System;
using System.Linq;

namespace Messages.ViewModels
{
    public class StatusBarViewModel : ViewModelBase
    {
        private readonly ILoggerCollection _loggerCollection;
        private ILogMessage _message;

        public StatusBarViewModel(ILoggerCollection loggerCollection)
        {
            _loggerCollection = loggerCollection ?? throw new ArgumentNullException(nameof(loggerCollection));

            Message = _loggerCollection.Messages.LastOrDefault();
            _loggerCollection.NewMessage += LoggerCollection_NewMessage;
        }

        private void LoggerCollection_NewMessage(object sender, Infrastructure.AppEventArgs.LoggerCollectionEventArgs e)
        {
            Message = e.Message;
        }

        public ILogMessage Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

    }
}
