using Infrastructure.AppEventArgs;
using Infrastructure.Interfaces.Logger;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;

namespace Messages.ViewModels
{
    public class MessagesViewModel : BindableBase
    {
        private readonly ILoggerCollection _loggerCollection;
        private string _message = "Test message";

        public MessagesViewModel(ILoggerCollection loggerCollection)
        {
            _loggerCollection = loggerCollection ?? throw new ArgumentNullException(nameof(loggerCollection));
            _loggerCollection.NewMessage += OnNewMessage;
            Data.AddRange(_loggerCollection.Messages);

            RemoveItemCommand = new DelegateCommand<ILogMessage>(ExecuteCommandName);
        }

        private void OnNewMessage(object _, LoggerCollectionEventArgs e)
        {
            Application.Current?.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => Data.Add(e.Message)));
        }
        void ExecuteCommandName(ILogMessage msg)
        {
            Data.Remove(msg);
        }

        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }
        public ObservableCollection<ILogMessage> Data { get; } = new ObservableCollection<ILogMessage>();

        public DelegateCommand<ILogMessage> RemoveItemCommand { get; }
    }
}
