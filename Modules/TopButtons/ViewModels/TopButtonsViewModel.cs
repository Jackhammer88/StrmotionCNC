using Infrastructure;
using Infrastructure.AggregatorEvents;
using Infrastructure.Constants;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Linq;

namespace TopButtons.ViewModels
{
    public class TopButtonsViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;
        private readonly IRegion _messageRegion;
        private readonly IUserSettingsService _settingsService;
        private readonly IControllerConfigurator _controllerConfigurator;
        private readonly IEventAggregator _eventAggregator;
        readonly IControllerConnectionManager _connectionManager;
        private readonly object _view;
        private bool _messagesActive;
        private int _feedrate;

        public TopButtonsViewModel(IRegionManager regionManager, IControllerConnectionManager connectionManager, IUserSettingsService settingsService, IControllerConfigurator controllerConfigurator, IEventAggregator eventAggregator)
        {
            _connectionManager = connectionManager;
            _settingsService = settingsService;
            _controllerConfigurator = controllerConfigurator;
            _eventAggregator = eventAggregator ?? throw new ArgumentNullException(nameof(eventAggregator));
            _regionManager = regionManager ?? throw new ArgumentNullException(nameof(regionManager));

            _eventAggregator.GetEvent<ConnectionEvent>().Subscribe(ReadFeedrateValue);

            MessageCommand = new DelegateCommand(ExecuteShowHideMessages);
            SettingsCommand = new DelegateCommand(ExecuteSettingsCommand);

            LaserPowerIncreaseCommand = new DelegateCommand(LaserPowerIncreaseExecute);
            LaserPowerDecreaseCommand = new DelegateCommand(LaserPowerDecreaseExecute);
            LaserOffCommand = new DelegateCommand(LaserOffExecute);
            LaserOnCommand = new DelegateCommand(LaserOnExecute);

            FeedrateMinusCommand = new DelegateCommand(FeedrateMinusExecute);
            FeedratePlusCommand = new DelegateCommand(FeedratePlusExecute);
            FeedrateOffCommand = new DelegateCommand(FeedrateOffExecute);
            Feedrate100Command = new DelegateCommand(Feedrate100Execute);

            _messageRegion = _regionManager.Regions[RegionNames.MessageRegion];
            _view = _messageRegion.Views.First();
            _messageRegion.Deactivate(_view);
            _messagesActive = false;

        }

        private void ReadFeedrateValue(bool connected)
        {
            if (connected)
                _controllerConfigurator.GetVariable(PVariables.FeedrateValue, out _feedrate);
        }

        private void Feedrate100Execute()
        {
            _feedrate = 100;
            _controllerConfigurator.SetVariable(PVariables.FeedrateValue, _feedrate);
        }

        private void FeedrateOffExecute()
        {
            _feedrate = 0;
            _controllerConfigurator.SetVariable(PVariables.FeedrateValue, _feedrate);
        }

        private void FeedrateMinusExecute()
        {
            if (_feedrate > 0)
            {
                _feedrate -= 10;
                _controllerConfigurator.SetVariable(PVariables.FeedrateValue, _feedrate);
            }
        }

        private void FeedratePlusExecute()
        {
            if (_feedrate < 120)
            {
                _feedrate += 10;
                _controllerConfigurator.SetVariable(PVariables.FeedrateValue, _feedrate);
            }
        }

        private void LaserOnExecute()
        {
        }

        private void LaserOffExecute()
        {
        }

        private void LaserPowerDecreaseExecute()
        {

        }

        private void LaserPowerIncreaseExecute()
        {

        }

        async void ExecuteSettingsCommand()
        {
            if (await _connectionManager.SelectControllerAsync(true).ConfigureAwait(false)) { }
        }
        void ExecuteShowHideMessages()
        {
            if (_messagesActive)
                _messageRegion.Deactivate(_view);
            else
                _messageRegion.Activate(_view);
            _messagesActive = !_messagesActive;
        }

        public DelegateCommand MessageCommand { get; }
        public DelegateCommand SettingsCommand { get; }
        public DelegateCommand LaserPowerIncreaseCommand { get; }
        public DelegateCommand LaserPowerDecreaseCommand { get; }
        public DelegateCommand LaserOffCommand { get; }
        public DelegateCommand LaserOnCommand { get; }
        public DelegateCommand FeedrateMinusCommand { get; }
        public DelegateCommand FeedratePlusCommand { get; }
        public DelegateCommand FeedrateOffCommand { get; }
        public DelegateCommand Feedrate100Command { get; }
    }
}
