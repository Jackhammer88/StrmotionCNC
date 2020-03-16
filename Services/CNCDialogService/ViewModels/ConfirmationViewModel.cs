using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace CNCDialogService.ViewModels
{
    public class ConfirmationViewModel : DialogBase
    {
        private string _message;

        public ConfirmationViewModel()
        {
            ConfirmCommand = new DelegateCommand<string>(ExecuteConfirmCommand);
        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            if (parameters == null)
                return;
            if (parameters.ContainsKey("title"))
                Title = parameters.GetValue<string>("title");
            if (parameters.ContainsKey("message"))
                Message = parameters.GetValue<string>("message");
        }
        void ExecuteConfirmCommand(string value)
        {
            ButtonResult result;
            if (value.Equals(CNCDialogServiceStrings.Yes, System.StringComparison.Ordinal))
                result = ButtonResult.Yes;
            else
                result = ButtonResult.No;
            RaiseRequestClose(new DialogResult(result));
        }

        public string Message
        {
            get => _message;
            private set => SetProperty(ref _message, value);
        }

        public DelegateCommand<string> ConfirmCommand { get; private set; }
    }
}
