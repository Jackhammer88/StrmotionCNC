using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using System;
using System.Globalization;

namespace GeneralComponents.ViewModels
{
    public class ManualExtViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        readonly IManualMachineControl _manualControl;
        private Velocity _selectedVelocity = Velocity.x1;
        private Axes _selectedAxis = Axes.X;

        public ManualExtViewModel(IEventAggregator eventAggregator, IManualMachineControl manualControl)
        {
            _manualControl = manualControl;
            _eventAggregator = eventAggregator;
            _eventAggregator?.GetEvent<MachineState>().Subscribe(MachineStateChanged);

            ChangeSelectedAxisCommand = new DelegateCommand<string>(ExecuteChangeSelectedAxisCommand);
            ChangeSelectedVelocityCommand = new DelegateCommand<string>(ExecuteChangeSelectedVelocityCommand);
            TryJogCommand = new DelegateCommand<string>(ExecuteTryJogCommand);
            StopJogCommand = new DelegateCommand(ExecuteStopJogCommand);
            JogIncrementallyCommand = new DelegateCommand<string>(ExecuteJogIncrementallyCommand);
            HomeCommand = new DelegateCommand(HomeExecute);

            Title = GeneralComponentsStrings.ManualExt;
        }

        private void HomeExecute()
        {
            _manualControl.Homing(Enum.GetName(typeof(Axes), _selectedAxis));
        }

        enum Axes
        {
            X = 1, Y, Z, A, B, C, U, V, W
        }
        enum Velocity
        {
            x1 = 1, x2, x3, x4, x5
        }

        private void MachineStateChanged(MachineStateType state)
        {
            IsManualStateSelected = state == MachineStateType.Manual;
        }
        void ExecuteChangeSelectedAxisCommand(string letter) => _selectedAxis = (Axes)Convert.ToInt32(letter, CultureInfo.InvariantCulture);
        void ExecuteChangeSelectedVelocityCommand(string rate) => _selectedVelocity = (Velocity)Convert.ToInt32(rate, CultureInfo.InvariantCulture);
        private void ExecuteTryJogCommand(string direction)
        {
            _manualControl.TryJog(Enum.GetName(typeof(Axes), _selectedAxis), direction.Equals("-", StringComparison.Ordinal), (int)_selectedVelocity);
        }
        private void ExecuteStopJogCommand()
        {
            _manualControl.StopJog();
        }
        private void ExecuteJogIncrementallyCommand(string direction)
        {
            _manualControl.JogIncrementally(Enum.GetName(typeof(Axes), _selectedAxis), EncoderUnits, direction.Equals("-", StringComparison.Ordinal), (int)_selectedVelocity);
        }

        private bool _isManualStateSelected = true;
        public bool IsManualStateSelected
        {
            get => _isManualStateSelected;
            set => SetProperty(ref _isManualStateSelected, value);
        }
        public int EncoderUnits { get; set; } = 100;

        public DelegateCommand<string> ChangeSelectedAxisCommand { get; }
        public DelegateCommand<string> ChangeSelectedVelocityCommand { get; }
        public DelegateCommand<string> TryJogCommand { get; }
        public DelegateCommand StopJogCommand { get; }
        public DelegateCommand<string> JogIncrementallyCommand { get; }
        public DelegateCommand HomeCommand { get; }
    }
}
