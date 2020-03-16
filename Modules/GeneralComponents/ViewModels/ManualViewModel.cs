using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using System;
using System.Globalization;
using System.Linq;

namespace GeneralComponents.ViewModels
{
    public class ManualViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        readonly IManualMachineControl _manualControl;
        private readonly IControllerConfigurator _configurator;
        private readonly IControllerInformation _controllerInformation;
        private bool _incrementalMode;

        public ManualViewModel(IEventAggregator eventAggregator, IManualMachineControl manualControl, IControllerConfigurator configurator, IControllerInformation controllerInformation)
        {
            _manualControl = manualControl;
            _configurator = configurator;
            _controllerInformation = controllerInformation;
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _eventAggregator.GetEvent<MachineState>().Subscribe(MachineStateChanged);

            TryJogCommand = new DelegateCommand<string>(ExecuteTryJogCommand);
            StopJogCommand = new DelegateCommand(ExecuteStopJogCommand);

            Title = GeneralComponentsStrings.Manual;
        }

        private void MachineStateChanged(MachineStateType state)
        {
            IsManualAvailable = (state == MachineStateType.Manual) || (state == MachineStateType.Test);
        }
        private void ExecuteTryJogCommand(string axis)
        {
            var seq = axis.Select(c => c.ToString(CultureInfo.InvariantCulture));

            if (IncrementalMode)
            {
                //_configurator.SetVariable(GetVariableAddresByAxis(axis), 1);
                _manualControl.JogIncrementally(seq.First(), CalculateUnits(seq.First(), Units), seq.Skip(1).First().Equals("-", StringComparison.Ordinal));
            }
            else
            {
                _currentAxis = axis;
                _configurator.SetVariable(GetVariableAddresByAxis(axis), 1);
                //_manualControl.TryJog(seq.First(), seq.Skip(1).First().Equals("-"));
            }
        }

        private static string GetVariableAddresByAxis(string axis)
        {
            var seq = axis.Select(c => c.ToString(CultureInfo.InvariantCulture));

            switch (axis)
            {
                case "X+":
                case "X-":
                    return seq.Skip(1).First().Equals("-", StringComparison.Ordinal) ? PVariables.PBJogMinusFirstMotor : PVariables.PBJogPlusFirstMotor;
                case "Y+":
                case "Y-":
                    return seq.Skip(1).First().Equals("-", StringComparison.Ordinal) ? PVariables.PBJogMinusSecondMotor : PVariables.PBJogPlusSecondMotor;
                default:
                    return seq.Skip(1).First().Equals("-", StringComparison.Ordinal) ? PVariables.PBJogMinusFirstMotor : PVariables.PBJogPlusFirstMotor;
            }
        }

        private int CalculateUnits(string axis, int Units)
        {
            var motor = _controllerInformation.Motors.FirstOrDefault(m => !string.IsNullOrEmpty(m.Letter) && m.Letter.Equals(axis, StringComparison.Ordinal));
            if (motor != null)
            {
                var index = _controllerInformation.Motors.IndexOf(motor);
                _configurator.GetVariable($"P800{5 + index}", out int resolution);
                var result = (int)Math.Ceiling(resolution * Units * 0.001);
                return result;
            }
            return 0;
        }

        private void ExecuteStopJogCommand()
        {
            if (!IncrementalMode)
            {
                //_manualControl.StopJog();
                _configurator.SetVariable(GetVariableAddresByAxis(_currentAxis), 0);
            }
        }

        private bool _isManualAvailable = true;
        private string _currentAxis;

        public bool IsManualAvailable
        {
            get => _isManualAvailable;
            set => SetProperty(ref _isManualAvailable, value);
        }
        public bool IncrementalMode
        {
            get => _incrementalMode;
            set => SetProperty(ref _incrementalMode, value);
        }
        public int Units { get; set; } = 1000;

        public DelegateCommand<string> TryJogCommand { get; }
        public DelegateCommand StopJogCommand { get; }
        public DelegateCommand<string> JogIncrementallyCommand { get; }
    }
}
