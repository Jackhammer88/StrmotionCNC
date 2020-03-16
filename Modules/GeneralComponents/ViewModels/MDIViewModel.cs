using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using System;
using System.Threading.Tasks;

namespace GeneralComponents.ViewModels
{
    public class MDIViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        readonly IProgramLoader _progamLoader;
        private string _editorText = string.Empty;
        private bool _buttonStartIsAvailable = true;
        private bool _isMdiStateSelected;

        public MDIViewModel(IEventAggregator eventAggregator, IProgramLoader progamLoader)
        {
            _progamLoader = progamLoader ?? throw new ArgumentNullException(nameof(progamLoader));
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _eventAggregator.GetEvent<MachineState>().Subscribe(state => IsMdiStateSelected = state == MachineStateType.MDI);
            Title = GeneralComponentsStrings.Mdi;
            StartProgramCommand = new DelegateCommand(ExecuteStartProgramCommand, CanExecuteStartProgramCommand);
            StopProgramCommand = new DelegateCommand(ExecuteStopProgramCommand);
            ClearCommand = new DelegateCommand(ExecuteClearCommand);

            _progamLoader.PropertyChanged += (s, e) =>
            {
                if (e == null) throw new NullReferenceException();
                if (e.PropertyName.Equals(nameof(_progamLoader.IsProgramRunning), StringComparison.Ordinal))
                {
                    ButtonStartIsAvailable = !_progamLoader.IsProgramRunning;
                    StartProgramCommand.RaiseCanExecuteChanged();
                }
            };
        }

        public bool IsMdiStateSelected
        {
            get => _isMdiStateSelected;
            private set => SetProperty(ref _isMdiStateSelected, value);
        }
        public string EditorText
        {
            get { return _editorText; }
            set
            {
                SetProperty(ref _editorText, value);
                StartProgramCommand.RaiseCanExecuteChanged();
            }
        }
        public bool ButtonStartIsAvailable
        {
            get => _buttonStartIsAvailable;
            set => SetProperty(ref _buttonStartIsAvailable, value);
        }

        bool CanExecuteStartProgramCommand()
        {
            return EditorText.Length > 0 && !_progamLoader.IsProgramRunning;
        }
        private async void ExecuteStartProgramCommand()
        {
            await Task.Run(() =>
            {
                ButtonStartIsAvailable = false;
                _progamLoader.LoadMDIProgram(EditorText.Replace("\r", string.Empty).Split('\n'));
            }).ConfigureAwait(true);
        }
        private void ExecuteStopProgramCommand()
        {
            _progamLoader.AbortMdiProgram();
            ButtonStartIsAvailable = true;
        }
        private void ExecuteClearCommand()
        {
            StopProgramCommand.Execute();
            EditorText = string.Empty;
        }

        public DelegateCommand StartProgramCommand { get; private set; }
        public DelegateCommand StopProgramCommand { get; private set; }
        public DelegateCommand ClearCommand { get; private set; }
    }
}
