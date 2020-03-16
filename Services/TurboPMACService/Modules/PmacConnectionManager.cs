using Infrastructure.AppEventArgs;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.Interfaces.UserSettingService;
using Prism.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ControllerService.Modules
{
    public class PmacConnectionManager : IControllerConnectionManager
    {
        readonly IController _controller;
        readonly IUserSettingsService _settings;
        readonly ILoggerFacade _logger;
        public PmacConnectionManager(IController controller, IUserSettingsService settings, ILoggerFacade logger)
        {
            _logger = logger;
            _settings = settings;
            _controller = controller ?? throw new NullReferenceException();

            _controller.OnLostConnection += Controller_OnLostConnection;
        }

        private void Controller_OnLostConnection(object sender, ControllerLostConnectionEventArgs e)
        {
            OnLostConnection?.Invoke(sender, e);
            IsReconnectionStarted = true;
        }

        ~PmacConnectionManager()
        {
            Disconnect();
        }

        public bool Connected => _controller.Connected;
        private bool _isReconnectionStarted;
        public bool IsReconnectionStarted
        {
            get => _isReconnectionStarted;
            private set
            {
                _isReconnectionStarted = value;
                if (value == true)
                    DoReconnect();
            }
        }
        public async Task ConnectAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() => Connect(), cancellationToken).ConfigureAwait(false);
        }
        /// <summary>
        /// Try connect to controller
        /// </summary>
        public void Connect()
        {
            if (_settings.DeviceNumber == -1)
                SelectControllerAsync().Wait();
            _controller.Connect(_settings.DeviceNumber, _settings.Timeout);
        }
        private void DoReconnect()
        {
            for (int i = 0; i < _settings.ReconnectionCount; i++)
            {
                Task.Delay(5000).Wait();
                if (!Connected)
                {
                    _logger.Log($"Try reconnect to controller #{i + 1}", Category.Info, Priority.Medium);
                    Connect();
                    if (Connected)
                    {
                        IsReconnectionStarted = false;
                        break;
                    }
                }
            }
        }
        /// <summary>
        /// Breaks the connection to the controller.
        /// </summary>
        public void Disconnect()
        {
            _controller.Disconnect();
        }

        /// <summary>
        /// Invites the user to select a controller
        /// </summary>
        public async Task<bool> SelectControllerAsync(bool autoConnect = false)
        {
            if (!_controller.SelectController(out int deviceNumber))
                return false;
            _settings.DeviceNumber = deviceNumber;
            if (autoConnect)
            {
                if (Connected)
                    Disconnect();
                await ConnectAsync(CancellationToken.None).ConfigureAwait(false);
            }

            return true;
        }

        public event EventHandler OnConnect
        {
            add { _controller.OnConnect += value; }
            remove { _controller.OnConnect -= value; }
        }
        public event EventHandler<ControllerErrorEventArgs> OnError
        {
            add { _controller.OnError += value; }
            remove { _controller.OnError -= value; }
        }
        public event EventHandler OnDisconnected
        {
            add { _controller.OnDisconnected += value; }
            remove { _controller.OnDisconnected -= value; }
        }

        public event EventHandler<ControllerLostConnectionEventArgs> OnLostConnection;
    }
}
