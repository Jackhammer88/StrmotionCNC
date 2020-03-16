using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Services.Dialogs;

namespace CNCDialogService.ViewModels
{
    public class GotoLineViewModel : DialogBase
    {
        private uint _lineNumber = 1;
        public uint LineNumber
        {
            get => _lineNumber;
            set => SetProperty(ref _lineNumber, value);
        }

        public GotoLineViewModel()
        {
            Title = CNCDialogServiceStrings.GoToLine;

            GotoCommand = new DelegateCommand(OnGotoExecute);
            CancelCommand = new DelegateCommand(OnCancelExecute);
        }

        void OnGotoExecute()
        {
            var result = new DialogResult(ButtonResult.OK);
            result.Parameters.Add("line", LineNumber);
            RaiseRequestClose(result);
        }
        void OnCancelExecute()
        {
            var result = new DialogResult(ButtonResult.Cancel);
            RaiseRequestClose(result);
        }

        public DelegateCommand GotoCommand { get; private set; }
        public DelegateCommand CancelCommand { get; private set; }
    }
}
