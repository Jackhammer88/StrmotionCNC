using Infrastructure.Abstract;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using System;

namespace GeneralComponents.ViewModels
{
    public class TerminalViewModel : ViewModelBase
    {
        readonly ITerminalClient _terminalClient;
        private string _textBoxText;
        private string _text;

        public TerminalViewModel(ITerminalClient terminalClient)
        {
            _terminalClient = terminalClient;

            Title = GeneralComponentsStrings.Terminal;
            SendToTerminalCommand = new DelegateCommand<string>(ExecuteSendToTerminalCommand);
        }

        private void ExecuteSendToTerminalCommand(string query)
        {
            _terminalClient.TerminalResponse(query, out string answer);
            answer = answer.Replace("\a", string.Empty);
            Text += query + Environment.NewLine + $"{answer}" + Environment.NewLine;
            TextBoxText = string.Empty;
            RaisePropertyChanged(nameof(TextBoxText));
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        public string TextBoxText
        {
            get => _textBoxText;
            set => SetProperty(ref _textBoxText, value);
        }

        public DelegateCommand<string> SendToTerminalCommand
        {
            get;
            private set;
        }
    }
}
