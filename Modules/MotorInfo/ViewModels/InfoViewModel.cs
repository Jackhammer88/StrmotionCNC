using Infrastructure.AggregatorEvents;
using Infrastructure.AppEventArgs;
using Infrastructure.DialogService;
using Infrastructure.Enums;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Resources.Strings;
using Prism.Commands;
using Prism.Events;
using Prism.Logging;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;

namespace MotorInfo.ViewModels
{
    public class InfoViewModel : BindableBase
    {
        readonly IControllerInformation _controllerInformation;
        readonly IControllerConnectionManager _controllerConnectionManager;
        readonly ILoggerFacade _logger;
        private readonly IDialogService _dialogService;
        readonly IEventAggregator _eventAggregator;
        private string _text;
        private bool _homingSuccess;
        private bool _phasingNeeded;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        public InfoViewModel(IControllerInformation controllerInformation, IControllerConnectionManager controllerConnectionManager, IEventAggregator eventAggregator, ILoggerFacade logger, IDialogService dialogService)
        {
            _logger = logger;
            _dialogService = dialogService;
            _eventAggregator = eventAggregator;
            _controllerConnectionManager = controllerConnectionManager;
            _controllerInformation = controllerInformation ?? throw new ArgumentNullException(nameof(controllerInformation));

            ChangeCoordinateState = new DelegateCommand<string>(ExecuteChangeCoordinateState);

            Text = "Test";
            RegisterCallbacks();
            ConnectToController();
            _controllerInformation.ActivatedMotorsCountChanged += ControllerInformation_ActivatedMotorsCountChanged;
        }

        private void RegisterCallbacks()
        {
            _controllerConnectionManager.OnConnect += OnConnect;
            _controllerConnectionManager.OnDisconnected += OnDisconnected;
            _controllerConnectionManager.OnError += OnError;
            _controllerConnectionManager.OnLostConnection += OnLostConnection;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void OnConnect(object sender, EventArgs e)
        {
            _logger.Log("Connected to controller", Category.Info, Priority.Medium);
            _eventAggregator.GetEvent<ConnectionEvent>().Publish(true);
            if (!_homingSuccess)
            {
                _homingSuccess = true;
                Application.Current.Dispatcher.Invoke(() =>
                {
                    _dialogService.Show(DialogNames.Confirmation, new DialogParameters { { "title", "" }, { "message", CNCDialogServiceStrings.WantHomingMessage } }, (r) =>
                    {
                        if (r.Result == ButtonResult.Yes)
                            MakeHoming();
                    });
                });
            }
        }

        private static void MakeHoming()
        {
            //TODO: Homing

        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        private void OnDisconnected(object sender, EventArgs e)
        {
            _logger.Log("Disconnected from controller", Category.Info, Priority.Medium);
            _eventAggregator.GetEvent<ConnectionEvent>().Publish(false);
        }
        private void OnLostConnection(object _, ControllerLostConnectionEventArgs e)
        {
            if (!_controllerConnectionManager.Connected)
            {
                _logger.Log($"Connection lost. Code: {e.Code}", Category.Info, Priority.High);
                _eventAggregator.GetEvent<ConnectionEvent>().Publish(false);
            }
        }
        private void OnError(object sender, ControllerErrorEventArgs e)
        {
            _logger.Log($"{e.Message}. Code: {e.ErrorCode}", Category.Warn, Priority.High);
        }
        private void ControllerInformation_ActivatedMotorsCountChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(ActiveMotorCount));
            RaisePropertyChanged(nameof(AxisData));
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Не перехватывать исключения общих типов", Justification = "<Ожидание>")]
        public async void ConnectToController()
        {
            try
            {
                await _controllerConnectionManager.ConnectAsync(CancellationToken.None).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.Log($"Connection error: {ex.Message}", Category.Exception, Priority.High);
            }
        }
        void ExecuteChangeCoordinateState(string arg)
        {
            var positionTypeNumber = int.Parse(arg, CultureInfo.InvariantCulture);
            _controllerInformation.SelectedPositionType = (PositionType)positionTypeNumber;
        }

        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }
        public List<IMotor> AxisData
        {
            get
            {
                var availableMotors = _controllerInformation.Motors.Where(m => m.Activated && !string.IsNullOrEmpty(m.Letter)).OrderBy(m => m.Letter);
                var result = new List<IMotor>();
                foreach (var motor in availableMotors)
                {
                    result.Add(motor);
                }
                return result;
            }
        }
        public int ActiveMotorCount
        {
            get => AxisData.Where(m => m.Activated).Count();
        }
        public bool PhasingNeeded
        {
            get => _phasingNeeded;
            set => SetProperty(ref _phasingNeeded, value);
        }

        public DelegateCommand<string> ChangeCoordinateState { get; }
    }
}
