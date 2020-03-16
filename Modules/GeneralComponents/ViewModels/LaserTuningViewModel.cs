using Infrastructure.Abstract;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using System;
using System.Runtime.CompilerServices;

namespace GeneralComponents.ViewModels
{
    public class LaserTuningViewModel : ViewModelBase
    {
        private int _minimalCuttingPower;
        private int _maximalCuttingPower;
        private int _cuttingSpeed;
        private int _gravSpeed;
        private int _minimalGravPower;
        private int _maximalGravPower;


        private bool _hasChange;
        private int _y2Offset;
        private readonly IControllerConfigurator _controllerConfigurator;
        private readonly ILoggerFacade _logger;
        private readonly IEventAggregator _eventAggregator;
        private readonly IProgramLoader _programLoader;

        public LaserTuningViewModel(IControllerConfigurator controllerConfigurator, ILoggerFacade logger, IEventAggregator eventAggregator, IProgramLoader programLoader)
        {
            _controllerConfigurator = controllerConfigurator;
            _logger = logger;
            _eventAggregator = eventAggregator ?? throw new NullReferenceException();
            _programLoader = programLoader ?? throw new NullReferenceException();
            Title = GeneralComponentsStrings.Laser;

            UploadCommand = new DelegateCommand(UploadExecute, UploadCanExecute);
            ShootOnCommand = new DelegateCommand(ShootOnExecute);
            ShootOffCommand = new DelegateCommand(ShootOffExecute);

            _eventAggregator.GetEvent<ConnectionEvent>().Subscribe(ReadLaserParameters);

            ChangingIsAvailable = true;
            _programLoader.PropertyChanged += ProgramLoader_PropertyChanged;
        }

        private void ProgramLoader_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(nameof(_programLoader.IsProgramRunning), StringComparison.Ordinal))
                ChangingIsAvailable = !_programLoader.IsProgramRunning;
        }

        private void ReadLaserParameters(bool connectionStatus)
        {
            if (!connectionStatus) return;
            _controllerConfigurator.GetVariable(PVariables.MinimalCuttingPower, out int minimalCuttingPower);
            MinimalCuttingPower = minimalCuttingPower;
            _controllerConfigurator.GetVariable(PVariables.MaximalCuttingPower, out int maximalCuttingPower);
            MaximalCuttingPower = maximalCuttingPower;
            _controllerConfigurator.GetVariable(PVariables.CuttingSpeed, out int cuttingSpeed);
            CuttingSpeed = cuttingSpeed;
            _controllerConfigurator.GetVariable(PVariables.GravSpeed, out int gravSpeed);
            GravSpeed = gravSpeed;
            _controllerConfigurator.GetVariable(PVariables.MinimalGravPower, out int minimalGravPower);
            MinimalGravPower = minimalGravPower;
            _controllerConfigurator.GetVariable(PVariables.MaximalGravPower, out int maximalGravPower);
            MaximalGravPower = maximalGravPower;
            _controllerConfigurator.GetVariable(PVariables.Y2Offset, out int y2Offset);
            Y2Offset = y2Offset;
            _controllerConfigurator.GetVariable(PVariables.BurnTime, out int burnTime);
            BurnTime = burnTime;

            _hasChange = false;
            UploadCommand.RaiseCanExecuteChanged();
        }

        private void ShootOffExecute()
        {
            Observe(_controllerConfigurator.SetVariable, PVariables.Shooting, 0);
        }

        private void ShootOnExecute()
        {
            Observe(_controllerConfigurator.SetVariable, PVariables.Shooting, 1);
        }

        private void UploadExecute()
        {
            UploadLaserParametersToController();
            _hasChange = false;
            UploadCommand.RaiseCanExecuteChanged();
        }

        private void UploadLaserParametersToController()
        {
            Observe(_controllerConfigurator.SetVariable, PVariables.MinimalCuttingPower, MinimalCuttingPower);
            Observe(_controllerConfigurator.SetVariable, PVariables.MaximalCuttingPower, MaximalCuttingPower);
            Observe(_controllerConfigurator.SetVariable, PVariables.CuttingSpeed, CuttingSpeed);
            Observe(_controllerConfigurator.SetVariable, PVariables.GravSpeed, GravSpeed);
            Observe(_controllerConfigurator.SetVariable, PVariables.MinimalGravPower, MinimalGravPower);
            Observe(_controllerConfigurator.SetVariable, PVariables.MaximalGravPower, MaximalGravPower);
            Observe(_controllerConfigurator.SetVariable, PVariables.Y2Offset, Y2Offset);
            Observe(_controllerConfigurator.SetVariable, PVariables.BurnTime, BurnTime);
        }

        private void Observe(Func<string, int, bool> action, string address, int value)
        {
            if (!action(address, value))
                _logger.Log($"Error when set {address} to {value}.", Category.Warn, Priority.High);
        }

        private bool UploadCanExecute()
        {
            return _hasChange;
        }

        public int MinimalCuttingPower
        {
            get => _minimalCuttingPower;
            set => Set(ref _minimalCuttingPower, value);
        }
        public int MaximalCuttingPower
        {
            get => _maximalCuttingPower;
            set => Set(ref _maximalCuttingPower, value);
        }
        public int CuttingSpeed
        {
            get => _cuttingSpeed;
            set => Set(ref _cuttingSpeed, value);
        }
        public int GravSpeed
        {
            get => _gravSpeed;
            set => Set(ref _gravSpeed, value);
        }
        public int MinimalGravPower
        {
            get => _minimalGravPower;
            set => Set(ref _minimalGravPower, value);
        }
        public int MaximalGravPower
        {
            get => _maximalGravPower;
            set => Set(ref _maximalGravPower, value);
        }
        public int Y2Offset
        {
            get => _y2Offset;
            set => Set(ref _y2Offset, value);
        }
        private int _burnTime;
        private bool _changingIsAvailable;

        public int BurnTime
        {
            get => _burnTime;
            set => Set(ref _burnTime, value);
        }


        private void Set(ref int internalValue, int value, [CallerMemberName]string propertyName = "")
        {
            SetProperty(ref internalValue, value, propertyName);
            _hasChange = true;
            UploadCommand.RaiseCanExecuteChanged();
        }
        public bool ChangingIsAvailable
        {
            get => _changingIsAvailable;
            private set => SetProperty(ref _changingIsAvailable, value);
        }

        public DelegateCommand UploadCommand { get; }
        public DelegateCommand ShootOnCommand { get; }
        public DelegateCommand ShootOffCommand { get; }
    }
}
