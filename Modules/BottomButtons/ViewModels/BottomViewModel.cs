using BottomButtons.Model;
using Infrastructure.AggregatorEvents;
using Infrastructure.AppEventArgs;
using Infrastructure.ApplicationStates;
using Infrastructure.Constants;
using Infrastructure.DialogService;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.ControlPanelService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;

namespace BottomButtons.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1060:Переместите вызовы PInvoke в класс собственных методов", Justification = "<Ожидание>")]
    public class BottomViewModel : BindableBase
    {
        private readonly IEventAggregator _aggregator;
        //private readonly IControlPanel _panel;
        readonly IProgramLoader _programLoader;
        readonly IDialogService _dialogService;
        private readonly IControllerConfigurator _controllerConfigurator;
        private bool _machineIsLocked;
        ProgramStatus _programStatus = new ProgramStatus();
        private bool _startPressed;
        private bool _isMDI;
        private bool _isAuto;
        private bool _isManual;
        private bool _isTest;
        private DelegateCommand _pauseCommand;
        private DelegateCommand _resumeCommand;
        private DelegateCommand _currentCommand;

        public BottomViewModel(IEventAggregator aggregator/*, IControlPanel controlPanel*/, IProgramLoader programLoader, IDialogService dialogService, IControllerConfigurator controllerConfigurator)
        {
            _dialogService = dialogService;
            _controllerConfigurator = controllerConfigurator;
            _programLoader = programLoader == null ? throw new NullReferenceException() : programLoader;
            _aggregator = aggregator;
            //_panel = controlPanel;
            _aggregator?.GetEvent<ConnectionEvent>().Subscribe(OnChangeConnectionState);
            _aggregator?.GetEvent<MachineState>().Subscribe(OnMachineStateChanged);
            _aggregator?.GetEvent<MachineLockedState>().Subscribe(OnMachineLocked);

            SetCommands();

            _programStatus.OnStateChanged += ProgramStatus_OnStateChanged;
            _programLoader.PropertyChanged += OnProgramRunned;
        }

        private void SetCommands()
        {
            StartCycleCommand = new DelegateCommand(StartCycleExecute, StartCycleCanExecute);
            _pauseCommand = new DelegateCommand(ExecutePauseCommand, () => !MachineIsLocked);
            _resumeCommand = new DelegateCommand(ExecuteResumeCommand, () => !MachineIsLocked);
            AbortCommand = new DelegateCommand(ExecuteAbortCommand, () => !MachineIsLocked);
            ChangeMachineState = new DelegateCommand<string>(ExecuteChangeMachineState);
            ShowKeyboardCommand = new DelegateCommand(ShowKeyboardExecute);
            ResetProgramCommand = new DelegateCommand(ExecuteResetCommand);
            UserSettingsCommand = new DelegateCommand(UserSettingsExecute);
            _currentCommand = _pauseCommand;
            ExitCommand = new DelegateCommand(ExecuteExitCommand);
        }

        private void UserSettingsExecute()
        {
            _dialogService.ShowDialog("UserSettings", new DialogParameters(), (t) => { });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Идентификаторы не должны содержать символы подчеркивания", Justification = "<Ожидание>")]
        public const Int32 WM_USER = 1024;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1707:Идентификаторы не должны содержать символы подчеркивания", Justification = "<Ожидание>")]
        public const Int32 WM_CSKEYBOARD = WM_USER + 192;
        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string _ClassName, string _WindowName);
        [DllImport("User32.DLL")]
        private static extern Boolean PostMessage(IntPtr hWnd, Int32 Msg, Int32 wParam, Int32 lParam);
        private async void ShowKeyboardExecute()
        {
            IntPtr hWnd = FindWindow("TFirstForm", "hvkFirstForm");
            await Task.Run(() => PostMessage(hWnd, WM_CSKEYBOARD, 1, 0)).ConfigureAwait(false);
        }

        private void OnMachineLocked(bool locked)
        {
            MachineIsLocked = locked;
            StartCycleCommand.RaiseCanExecuteChanged();
            _pauseCommand.RaiseCanExecuteChanged();
            _resumeCommand.RaiseCanExecuteChanged();
            AbortCommand.RaiseCanExecuteChanged();
        }
        private void ProgramStatus_OnStateChanged(object _, ProgramStateEventArgs args)
        {
            if (args.State == ProgramState.ProgramIsRunning)
            {
                StartCycleCommand.RaiseCanExecuteChanged();
                PauseCommand = _pauseCommand;
                StartPressed = false;
            }
            if (args.State == ProgramState.ProgramPaused)
            {
                if (_programLoader.IsProgramPaused)
                    PauseCommand = _resumeCommand;
            }
            if (args.State == ProgramState.ProgramStopped)
            {
                StartCycleCommand.RaiseCanExecuteChanged();
                PauseCommand = _pauseCommand;
            }
            RaisePropertyChanged(nameof(IsProgramPaused));
        }
        private void OnChangeConnectionState(bool obj)
        {
            if (!obj) return;
            if (GetInternalState(out int internalState))
                ExecuteChangeMachineState(internalState.ToString(CultureInfo.InvariantCulture));
            else
                ExecuteChangeMachineState("2");

        }
        private void OnProgramRunned(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramRunning), StringComparison.Ordinal))
            {
                _programStatus.CurrentState = _programLoader.IsProgramRunning ? ProgramState.ProgramIsRunning : ProgramState.ProgramStopped;
            }
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramPaused), StringComparison.Ordinal))
            {
                _programStatus.CurrentState = _programLoader.IsProgramPaused ? ProgramState.ProgramPaused : ProgramState.ProgramIsRunning;
            }
        }
        void OnMachineStateChanged(MachineStateType state)
        {
            if (state == MachineStateType.Auto)
            {
                //TODO: загрузить программу и приготовить её к запуску
                _programLoader.PrepareProgramAsync();

            }
            if (_programLoader.IsProgramRunning)
            {
                _programLoader.ExitFromAuto();
            }
        }
        void ExecuteChangeMachineState(string val)
        {
            GetInternalState(out int machineInternalState);
            MachineStateType oldState = (MachineStateType)machineInternalState;
            MachineStateType state = (MachineStateType)Convert.ToInt32(val, CultureInfo.InvariantCulture);
            if (oldState == MachineStateType.Auto)
            {
                _programLoader.ExitFromAuto();
            }
            TryToChangeStateInternal(state);
            if (GetInternalState(out machineInternalState) && machineInternalState == int.Parse(val, CultureInfo.InvariantCulture))
            {
                if (state == MachineStateType.Auto && !_programLoader.IsProgramOpened)
                {
                    IsManual = true;
                    IsAuto = false;
                    state = MachineStateType.Manual;
                }
                DoChangeMachineState(state);
                TryToChangeStateInternal(state);
            }
        }

        private bool GetInternalState(out int machineInternalState)
        {
            return _controllerConfigurator.GetVariable(PVariables.MachineStateInternal, out machineInternalState);
        }

        private void TryToChangeStateInternal(MachineStateType state)
        {
            switch (state)
            {
                case MachineStateType.Auto:
                    _controllerConfigurator.SetVariable(PVariables.PBAutoButtonState, 1);
                    break;
                case MachineStateType.Manual:
                    _controllerConfigurator.SetVariable(PVariables.PManualButtonState, 1);
                    break;
                case MachineStateType.MDI:
                    _controllerConfigurator.SetVariable(PVariables.PMDIButtonState, 1);
                    break;
                case MachineStateType.Test:
                    _controllerConfigurator.SetVariable(PVariables.PBTestButtonState, 1);
                    break;
                case MachineStateType.Unknown:
                default:
                    break;
            }
        }

        private void DoChangeMachineState(MachineStateType state)
        {
            switch (state)
            {
                case MachineStateType.Auto:
                    IsAuto = true;
                    IsMDI = IsManual = IsTest = false;
                    //_panel.SetLed(1, true);
                    //_panel.SetLed(2, false);
                    //_panel.SetLed(3, false);
                    //_panel.SetLed(4, false);
                    break;
                case MachineStateType.MDI:
                    IsMDI = true;
                    IsAuto = IsManual = IsTest = false;
                    //_panel.SetLed(1, false);
                    //_panel.SetLed(2, true);
                    //_panel.SetLed(3, false);
                    //_panel.SetLed(4, false);
                    break;
                case MachineStateType.Manual:
                    IsManual = true;
                    IsAuto = IsMDI = IsTest = false;
                    //_panel.SetLed(1, false);
                    //_panel.SetLed(2, false);
                    //_panel.SetLed(3, true);
                    //_panel.SetLed(4, false);
                    break;
                case MachineStateType.Test:
                    IsTest = true;
                    IsAuto = IsMDI = IsManual = false;
                    //_panel.SetLed(1, false);
                    //_panel.SetLed(2, false);
                    //_panel.SetLed(3, false);
                    //_panel.SetLed(4, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(CommonStrings.UnacceptableMachineState);
            }
            _aggregator.GetEvent<MachineState>().Publish(state);
        }
        private bool StartCycleCanExecute() => !IsProgramRunning && !MachineIsLocked;
        private void StartCycleExecute()
        {
            _programLoader.CycleStart();
        }
        void ExecuteAbortCommand()
        {
            _programLoader.AbortProgram();
            StartCycleCommand.RaiseCanExecuteChanged();
        }
        private void ExecuteResumeCommand()
        {
            _programLoader.ResumeProgram();
        }
        void ExecutePauseCommand()
        {
            _programLoader.PauseProgram();
        }
        private void ExecuteResetCommand()
        {
            _programLoader.ResetProgram();
            _aggregator.GetEvent<ResetEvent>().Publish();
            DoChangeMachineState(MachineStateType.Manual);
        }
        void ExecuteExitCommand()
        {

            var title = BottomButtonsStrings.ExitConfirmationTitle;
            var message = BottomButtonsStrings.ExitConfirmationMessage;
            var parameters = new DialogParameters
            {
                { "title", title },
                { "message", message }
            };
            _dialogService.ShowDialog(DialogNames.Confirmation, parameters, r =>
            {
                if (r.Result == ButtonResult.Yes)
                    Application.Current.Shutdown();
            });
        }

        public bool StartPressed
        {
            get => _startPressed;
            set => SetProperty(ref _startPressed, value);
        }
        public bool IsAuto
        {
            get => _isAuto;
            set
            {
                SetProperty(ref _isAuto, value);
                if (value)
                    IsManual = IsMDI = IsTest = false;
            }
        }
        public bool IsMDI
        {
            get => _isMDI;
            set
            {
                SetProperty(ref _isMDI, value);
                if (value)
                    IsAuto = IsManual = IsTest = false;
            }
        }
        public bool IsManual
        {
            get => _isManual;
            set
            {
                SetProperty(ref _isManual, value);
                if (value)
                    IsAuto = IsMDI = IsTest = false;
            }
        }
        public bool IsTest
        {
            get => _isTest;
            set
            {
                SetProperty(ref _isTest, value);
                if (value)
                    IsAuto = IsMDI = IsManual = false;
            }
        }
        public bool IsProgramRunning => _programLoader.IsProgramRunning;
        public bool IsProgramPaused => _programLoader.IsProgramPaused;
        public bool MachineIsLocked
        {
            get => _machineIsLocked;
            set => SetProperty(ref _machineIsLocked, value);
        }

        public DelegateCommand StartCycleCommand { get; private set; }
        public DelegateCommand PauseCommand
        {
            get => _currentCommand;
            private set => SetProperty(ref _currentCommand, value);
        }
        public DelegateCommand AbortCommand { get; private set; }
        public DelegateCommand ResetProgramCommand { get; private set; }
        public DelegateCommand UserSettingsCommand { get; private set; }
        public DelegateCommand<string> ChangeMachineState { get; private set; }
        public DelegateCommand ShowKeyboardCommand { get; private set; }
        public DelegateCommand ExitCommand { get; private set; }
    }
}
